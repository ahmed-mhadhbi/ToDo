import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private _accessToken = new BehaviorSubject<string | null>(null);
  accessToken$ = this._accessToken.asObservable();
  private REFRESH = 'refresh_token';

  token: any;

   constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('accessToken');
      this._accessToken.next(token);
    }
  }

  save(accessToken: string, refreshToken: string) {
    if(isPlatformBrowser(this.platformId))
      {
    localStorage.setItem('_accessToken', accessToken);
    localStorage.setItem('REFRESH', refreshToken);
    }
     this._accessToken.next(accessToken);
  }

  getAccess() {
    if (isPlatformBrowser(this.platformId)) {
    return localStorage.getItem('accessToken');
     }
    return null;
  }

  getRefresh() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.REFRESH);
    }
    
    return null;
  }

  clear() {
     if (isPlatformBrowser(this.platformId)) {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
     }
    this._accessToken.next(null);

  }
}
