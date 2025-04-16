import {Component, Input, OnInit} from '@angular/core';
import {MediaService} from '../../services/media/media.service';
import {Media} from '../../types/media.type';
import {environment} from "../../../environments/environment";
import {HttpUrlEncodingCodec} from "@angular/common/http";
import {NgOptimizedImage} from "@angular/common";
import Swal from 'sweetalert2'

@Component({
  selector: 'app-media-grid',
  standalone: true,
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './media-grid.component.html',
})
export class MediaGridComponent {
  constructor(private mediaService: MediaService) {
  }

  @Input()
  public media: Media[] = [];

  public getThumbnailUrl(item: Media) {
    return item.thumbnailUrl ? environment.BACKEND_URL + "/media/thumbnail?path=" + new HttpUrlEncodingCodec().encodeValue(item.thumbnailUrl) : "assets/movie.png";
  }

  @Input()
  public changePathCallBack: (path: string) => void = () => {
  };

  public handleClick(item: Media) {
    if (item.type === 0) {
      this.changePathCallBack(item.path);
    } else {
      window.open(environment.BACKEND_URL + '/media/file?path=' + new HttpUrlEncodingCodec().encodeValue(item.path), '_blank');

    }
  }

  public copyLink(item: Media, $event: MouseEvent) {
    $event.stopPropagation()

    var path = environment.BACKEND_URL + '/media/file?path=' + new HttpUrlEncodingCodec().encodeValue(item.path);
    navigator.clipboard.writeText(path);
    Swal.fire({
      title: 'Link copied!',
      text: 'The download link has been copied!',
      icon: 'info',
      toast: true,
      position: "bottom-right",
      timer: 3000,
      showConfirmButton: false,
      timerProgressBar: true
    })

  }

}
