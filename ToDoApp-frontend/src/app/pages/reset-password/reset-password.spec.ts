import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResetPassword } from './reset-password';

 import { provideRouter } from '@angular/router';
 import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

beforeEach(async () => {
  await TestBed.configureTestingModule({
    imports: [ResetPassword],
    providers: [
      {
        provide: ActivatedRoute,
        useValue: {
          snapshot: {
            queryParamMap: {
              get: () => 'fake-token'
            }
          }
        }
      }
    ]
  }).compileComponents();
});

describe('ResetPassword', () => {
  let component: ResetPassword;
  let fixture: ComponentFixture<ResetPassword>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResetPassword]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResetPassword);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
