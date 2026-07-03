import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FavoritoService } from './favorito.service';
import { FavoritoDto } from './favorito.model';

@Component({
  selector: 'app-favoritos',
  templateUrl: './favoritos.component.html',
  imports: [CommonModule],
})
export class FavoritosComponent implements OnInit {
  favoritos: FavoritoDto[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private favoritoService: FavoritoService) {}

  ngOnInit(): void {
    this.loadFavoritos();
  }

  loadFavoritos(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.favoritoService.getMyFavoritos().subscribe({
      next: (result) => {
        this.favoritos = result;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'No se pudieron cargar los favoritos.';
        this.isLoading = false;
      },
    });
  }

  removeFavorito(destinoTuristicoId: string): void {
    this.favoritoService.removeFavorito(destinoTuristicoId).subscribe({
      next: () => {
        this.favoritos = this.favoritos.filter(
          (f) => f.destinoTuristicoId !== destinoTuristicoId
        );
      },
      error: () => {
        this.errorMessage = 'No se pudo eliminar el favorito.';
      },
    });
  }
}
