import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { DestinoTuristicoService } from '../../proxy/destinos-turisticos/destino-turistico.service';
import { CityInformationDto } from '../../proxy/destinos/models';
import { finalize } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CalificacionService } from '../../proxy/destinos-turisticos/calificacion.service';
import { CalificacionDto } from '../../proxy/destinos/models';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-destinos-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './destinos-detail.html',
  styleUrls: ['./destinos-detail.scss']
})
export class DestinosDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private destinationService = inject(DestinoTuristicoService);
  private location = inject(Location);
  private sanitizer = inject(DomSanitizer);
  private http = inject(HttpClient);
  private calificacionService = inject(CalificacionService);

  private configState = inject(ConfigStateService);

  cityId: number | null = null;
  localDestinoId: string | null = null;
  cityDetails: CityInformationDto | null = null;
  currentUserId: string | null = null;
  
  // Reseñas
  calificaciones: CalificacionDto[] = [];
  promedio: number = 0;
  nuevaPuntuacion: number = 5;
  nuevoComentario: string = '';
  enviandoResena: boolean = false;
  syncError: string = '';
  loading = true;
  errorMessage = '';
  mapUrl: SafeResourceUrl | null = null;

  // Edit Mode
  editingReviewId: string | null = null;
  editPuntuacion: number = 5;
  editComentario: string = '';
  enviandoEdicion: boolean = false;
  
  // Información enriquecida de Wikipedia
  cityDescription: string = '';
  cityImage: string = '';

  readonly defaultImage = 'assets/images/destination-placeholder.svg';

  ngOnInit(): void {
    const currentUser = this.configState.getOne('currentUser');
    if (currentUser) {
      this.currentUserId = currentUser.id;
    }

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      if (idParam.includes('-')) {
        // Es un GUID (Local Destino)
        this.loadLocalDestinoAndMockDetails(idParam);
      } else {
        // Es un ID de GeoDB
        this.cityId = parseInt(idParam, 10);
        if (!isNaN(this.cityId)) {
          this.loadDetails(this.cityId);
        } else {
          this.errorMessage = 'ID de destino inválido.';
          this.loading = false;
        }
      }
    }
  }

  loadLocalDestinoAndMockDetails(localId: string): void {
    this.loading = true;
    this.errorMessage = '';
    
    this.destinationService.get(localId).subscribe({
      next: (destino) => {
        this.localDestinoId = localId;
        // Mock the cityDetails from local Destino properties
        this.cityDetails = {
          id: 0,
          name: destino.nombre || 'Desconocido',
          country: 'Argentina', // Puede que no esté disponible sin joins
          region: 'N/A',
          population: destino.poblacion || 0,
          latitude: destino.latitud || 0,
          longitude: destino.longuitud || 0,
          timezone: 'N/A'
        } as any;

        const url = `https://maps.google.com/maps?q=${this.cityDetails!.latitude},${this.cityDetails!.longitude}&t=&z=13&ie=UTF8&iwloc=&output=embed`;
        this.mapUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);

        this.fetchWikipediaInfo(this.cityDetails!.name);
        this.loadReviews();
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'No se pudo cargar la información de este destino local.';
        this.loading = false;
      }
    });
  }

  loadDetails(id: number): void {
    this.loading = true;
    this.errorMessage = '';
    
    this.destinationService.getCityDetails(id)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (details) => {
          this.cityDetails = details;
          const url = `https://maps.google.com/maps?q=${details.latitude},${details.longitude}&t=&z=13&ie=UTF8&iwloc=&output=embed`;
          this.mapUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
          
          // Buscar información extra en Wikipedia
          this.fetchWikipediaInfo(details.name);
          
          // Sincronizar con DB local para obtener el Guid y cargar reseñas
          this.syncLocalDestinoAndLoadReviews(details);
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'No se pudo cargar la información de este destino en GeoDB.';
        }
      });
  }

  syncLocalDestinoAndLoadReviews(cityInfo: CityInformationDto): void {
    this.syncError = '';
    this.destinationService.syncDestinoLocal(cityInfo).subscribe({
      next: (localDestino) => {
        if (localDestino && localDestino.id) {
          this.localDestinoId = localDestino.id;
          this.loadReviews();
        } else {
          this.syncError = 'El backend no devolvió un ID válido para este destino.';
        }
      },
      error: (err) => {
        console.warn('Error al sincronizar destino local', err);
        this.syncError = 'Error de conexión con el backend para las calificaciones. Asegúrate de haber reiniciado el servidor C#.';
      }
    });
  }

  loadReviews(): void {
    if (!this.localDestinoId) return;

    this.calificacionService.listarComentarios(this.localDestinoId).subscribe(res => {
      this.calificaciones = res;
    });

    this.calificacionService.obtenerPromedio(this.localDestinoId).subscribe(res => {
      this.promedio = res;
    });
  }

  enviarResena(): void {
    if (!this.localDestinoId || !this.nuevoComentario.trim()) return;

    this.enviandoResena = true;
    this.calificacionService.crearCalificacion({
      destinoTuristicoId: this.localDestinoId,
      puntuacion: this.nuevaPuntuacion,
      comentario: this.nuevoComentario
    }).pipe(finalize(() => this.enviandoResena = false))
    .subscribe({
      next: () => {
        this.nuevoComentario = '';
        this.nuevaPuntuacion = 5;
        this.loadReviews(); // Recargar reseñas
      },
      error: (err) => {
        console.error('Error al enviar reseña', err);
        alert('Hubo un error al guardar tu reseña. Es posible que ya hayas dejado una reseña.');
      }
    });
  }

  iniciarEdicion(review: CalificacionDto): void {
    this.editingReviewId = review.id;
    this.editPuntuacion = review.puntuacion;
    this.editComentario = review.comentario;
  }

  cancelarEdicion(): void {
    this.editingReviewId = null;
  }

  guardarEdicion(): void {
    if (!this.localDestinoId || !this.editComentario.trim()) return;
    this.enviandoEdicion = true;
    this.calificacionService.editarCalificacion(this.localDestinoId, {
      destinoTuristicoId: this.localDestinoId,
      puntuacion: this.editPuntuacion,
      comentario: this.editComentario
    }).pipe(finalize(() => this.enviandoEdicion = false))
    .subscribe({
      next: () => {
        this.editingReviewId = null;
        this.loadReviews();
      },
      error: (err) => {
        console.error('Error al editar reseña', err);
        alert('Hubo un error al editar tu reseña.');
      }
    });
  }

  eliminarResena(): void {
    if (!this.localDestinoId) return;
    if (!confirm('¿Estás seguro de que quieres eliminar tu reseña?')) return;

    this.calificacionService.eliminarCalificacion(this.localDestinoId).subscribe({
      next: () => {
        this.loadReviews();
      },
      error: (err) => {
        console.error('Error al eliminar reseña', err);
        alert('Hubo un error al eliminar tu reseña.');
      }
    });
  }

  fetchWikipediaInfo(cityName: string): void {
    // Reemplaza espacios por guiones bajos para la URL de Wikipedia
    const query = cityName.trim().replace(/\s+/g, '_');
    const wikiUrl = `https://es.wikipedia.org/api/rest_v1/page/summary/${query}`;

    this.http.get<any>(wikiUrl).subscribe({
      next: (response) => {
        if (response && response.extract) {
          this.cityDescription = response.extract;
        }
        if (response && response.thumbnail && response.thumbnail.source) {
          this.cityImage = response.thumbnail.source;
        }
      },
      error: (err) => {
        // Ignoramos el error silenciosamente, simplemente no se mostrará info extra.
        console.warn('No se encontró información en Wikipedia para esta ciudad.');
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
  
  openInMaps(): void {
    if(this.cityDetails) {
      const url = `https://www.google.com/maps/search/?api=1&query=${this.cityDetails.latitude},${this.cityDetails.longitude}`;
      window.open(url, '_blank');
    }
  }
}
