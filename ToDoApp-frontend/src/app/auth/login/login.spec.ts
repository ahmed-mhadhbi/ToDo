import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Login } from './login';


import { provideRouter } from '@angular/router';
 import { ActivatedRoute } from '@angular/router';

 beforeEach(async () => {
  await TestBed.configureTestingModule({
    imports: [Login],
    providers: [
      provideRouter([]),
      {
        provide: ActivatedRoute,
        useValue: {
          snapshot: {
            paramMap: {
              get: () => null
            }
          }
        }
      }
    ]
  }).compileComponents();
});


describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Login]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
