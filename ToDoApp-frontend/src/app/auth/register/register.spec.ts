import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Register } from './register';

import { provideRouter } from '@angular/router';
 import { ActivatedRoute } from '@angular/router';

 beforeEach(async () => {
  await TestBed.configureTestingModule({
    imports: [Register],
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


describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Register]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
