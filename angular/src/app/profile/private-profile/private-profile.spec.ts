import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivateProfile } from './private-profile';

describe('PrivateProfile', () => {
  let component: PrivateProfile;
  let fixture: ComponentFixture<PrivateProfile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrivateProfile]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrivateProfile);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
