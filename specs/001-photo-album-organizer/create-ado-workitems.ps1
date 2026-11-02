param(
    [string]$OrganizationUrl = $env:ADO_ORG_URL,
    [string]$Project = $env:ADO_PROJECT,
    [string]$Pat = $env:ADO_PAT,
    [string]$InputPath = (Join-Path $PSScriptRoot 'azure-devops-workitems.json')
)

if (-not $OrganizationUrl -or -not $Project -or -not $Pat) {
    throw "Set ADO_ORG_URL, ADO_PROJECT, and ADO_PAT environment variables before running this script."
}

$ErrorActionPreference = 'Stop'
$baseUrl = $OrganizationUrl.TrimEnd('/')
$headers = @{ Authorization = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$Pat")) }

function Get-ExistingWorkItemByTitle {
    param([string]$Title)

    $query = @{
        query = "SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.TeamProject] = '$Project' AND [System.Title] = '$Title' ORDER BY [System.Id]"
    }

    $uri = "$baseUrl/$Project/_apis/wit/wiql?api-version=7.1"
    $result = Invoke-RestMethod -Uri $uri -Method Post -Headers $headers -ContentType 'application/json' -Body ($query | ConvertTo-Json -Depth 20)

    if ($result.workItems.Count -gt 0) {
        $existingId = $result.workItems[0].id
        $existingUrl = "$baseUrl/$Project/_apis/wit/workItems/$existingId"
        return @{ id = $existingId; url = $existingUrl }
    }

    return $null
}

function Resolve-AdoType {
    param([string]$Type)

    if ($Type -eq 'Feature') {
        return 'Task'
    }

    return $Type
}

function New-AdoWorkItem {
    param(
        [string]$Type,
        [string]$Title,
        [string]$Description,
        [string]$ParentUrl = $null
    )

    $resolvedType = Resolve-AdoType -Type $Type

    $body = @(
        @{ op = 'add'; path = '/fields/System.Title'; value = $Title },
        @{ op = 'add'; path = '/fields/System.Description'; value = $Description }
    )

    if ($ParentUrl) {
        $body += @{ op = 'add'; path = '/relations/-'; value = @{ rel = 'System.LinkTypes.Hierarchy-Reverse'; url = $ParentUrl } }
    }

    $uri = "$baseUrl/$Project/_apis/wit/workitems/$" + $resolvedType + "?api-version=7.1-preview.3"
    $result = Invoke-RestMethod -Uri $uri -Method Patch -Headers $headers -ContentType 'application/json-patch+json' -Body ($body | ConvertTo-Json -Depth 20)
    return $result
}

function ConvertTo-Description {
    param([object]$Item)

    $lines = @()
    if ($Item.description) { $lines += $Item.description }
    if ($Item.acceptanceCriteria) {
        $lines += ''
        $lines += 'Acceptance Criteria:'
        for ($i = 0; $i -lt $Item.acceptanceCriteria.Count; $i++) {
            $lines += "- $($Item.acceptanceCriteria[$i])"
        }
    }
    if ($Item.requirements) {
        $lines += ''
        $lines += 'Requirements:'
        foreach ($req in $Item.requirements) { $lines += "- $req" }
    }
    if ($Item.filePath) {
        $lines += ''
        $lines += "Implementation file(s): $($Item.filePath)"
    }
    return ($lines -join [Environment]::NewLine)
}

$manifest = Get-Content -Path $InputPath -Raw | ConvertFrom-Json

$created = @{}

function Create-Node {
    param(
        [object]$Node,
        [string]$ParentUrl = $null,
        [string]$ParentTitle = $null
    )

    $title = $Node.title
    $type = $Node.type

    $existing = Get-ExistingWorkItemByTitle -Title $title
    if ($existing) {
        Write-Host "Skipping duplicate $type :: $title (already exists as ID $($existing.id))"
        $item = @{ url = $existing.url; id = $existing.id }
    }
    else {
        Write-Host "Creating $type :: $title"
        $item = New-AdoWorkItem -Type $type -Title $title -Description (ConvertTo-Description -Item $Node) -ParentUrl $ParentUrl
    }

    if (-not $created.ContainsKey($title)) {
        $created[$title] = @{}
    }
    $created[$title]['url'] = $item.url
    $created[$title]['id'] = $item.id

    if ($Node.children) {
        foreach ($child in $Node.children) {
            Create-Node -Node $child -ParentUrl $item.url -ParentTitle $title
        }
    }
}

Create-Node -Node $manifest.feature

Write-Host "Created work items for $Project in $OrganizationUrl"
$created | ConvertTo-Json -Depth 20
