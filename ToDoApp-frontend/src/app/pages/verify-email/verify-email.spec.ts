 import { ComponentFixture, TestBed } from '@angular/core/testing';
 import { VerifyEmailComponent } from './verify-email';

 import { provideRouter } from '@angular/router';
 import { ActivatedRoute } from '@angular/router';

 beforeEach(async () => {
  await TestBed.configureTestingModule({
    imports: [VerifyEmailComponent],
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

 describe('VerifyEmailComponent', () => {
   let component: VerifyEmailComponent;
   let fixture: ComponentFixture<VerifyEmailComponent>;
   beforeEach(async () => {
     await TestBed.configureTestingModule({
       imports: [VerifyEmailComponent]
     })
     .compileComponents();

     fixture = TestBed.createComponent(VerifyEmailComponent);
     component = fixture.componentInstance;
     await fixture.whenStable();
   });

   it('should create', () => {
     expect(component).toBeTruthy();
  });
 });
