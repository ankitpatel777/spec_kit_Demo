import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AlbumEditorComponent } from './components/album-editor/album-editor.component';
import { AlbumTileComponent } from './components/album-tile/album-tile.component';
import { AlbumOverviewComponent } from './pages/album-overview/album-overview.component';

@NgModule({
  declarations: [AppComponent, AlbumOverviewComponent, AlbumTileComponent, AlbumEditorComponent],
  imports: [BrowserModule, FormsModule, HttpClientModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
