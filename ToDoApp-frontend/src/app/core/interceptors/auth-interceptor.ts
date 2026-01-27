import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { TokenService,  } from '../services/token';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';


export const authInterceptor: HttpInterceptorFn = (req, next) => {
   const tokenService = inject(TokenService);
   const platformId = inject(PLATFORM_ID);



    let token: string | null = null;
  if (isPlatformBrowser(platformId)) {
    token = tokenService.getAccess();
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    }
  }

  //console.log('JWT TOKEN:', token);
  return next(req);
};