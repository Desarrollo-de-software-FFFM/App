import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DestinationsListComponent } from './destinos-list';

describe('DestinosList', () => {
  let component: DestinationsListComponent;
  let fixture: ComponentFixture<DestinationsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DestinationsListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DestinationsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
