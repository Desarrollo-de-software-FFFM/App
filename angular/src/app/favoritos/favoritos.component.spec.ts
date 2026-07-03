import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { FavoritosComponent } from './favoritos.component';
import { FavoritoService } from './favorito.service';
import { FavoritoDto } from './favorito.model';

const MOCK_FAVORITO: FavoritoDto = {
  id: 'aaa-111',
  destinoTuristicoId: 'dest-001',
  userId: 'user-001',
  creationTime: new Date().toISOString(),
};

describe('FavoritosComponent', () => {
  let component: FavoritosComponent;
  let fixture: ComponentFixture<FavoritosComponent>;
  let favoritoServiceSpy: jasmine.SpyObj<FavoritoService>;

  beforeEach(async () => {
    favoritoServiceSpy = jasmine.createSpyObj('FavoritoService', [
      'getMyFavoritos',
      'removeFavorito',
    ]);

    await TestBed.configureTestingModule({
      imports: [FavoritosComponent],
      providers: [{ provide: FavoritoService, useValue: favoritoServiceSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(FavoritosComponent);
    component = fixture.componentInstance;
  });

  it('should load favoritos on init', () => {
    favoritoServiceSpy.getMyFavoritos.and.returnValue(of([MOCK_FAVORITO]));

    fixture.detectChanges();

    expect(component.favoritos.length).toBe(1);
    expect(component.favoritos[0].destinoTuristicoId).toBe('dest-001');
    expect(component.isLoading).toBeFalse();
  });

  it('should show empty state when no favoritos', () => {
    favoritoServiceSpy.getMyFavoritos.and.returnValue(of([]));

    fixture.detectChanges();

    expect(component.favoritos).toEqual([]);
    expect(component.isLoading).toBeFalse();
    expect(component.errorMessage).toBe('');
  });

  it('should set errorMessage on load failure', () => {
    favoritoServiceSpy.getMyFavoritos.and.returnValue(throwError(() => new Error('Error')));

    fixture.detectChanges();

    expect(component.errorMessage).toBeTruthy();
    expect(component.isLoading).toBeFalse();
  });

  it('should remove favorito from list after successful delete', () => {
    favoritoServiceSpy.getMyFavoritos.and.returnValue(of([MOCK_FAVORITO]));
    favoritoServiceSpy.removeFavorito.and.returnValue(of(void 0));

    fixture.detectChanges();
    component.removeFavorito('dest-001');

    expect(component.favoritos).toEqual([]);
  });

  it('should set errorMessage on delete failure', () => {
    favoritoServiceSpy.getMyFavoritos.and.returnValue(of([MOCK_FAVORITO]));
    favoritoServiceSpy.removeFavorito.and.returnValue(throwError(() => new Error('Error')));

    fixture.detectChanges();
    component.removeFavorito('dest-001');

    expect(component.errorMessage).toBeTruthy();
  });
});
