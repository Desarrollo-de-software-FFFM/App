import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UserService } from '../../proxy/usuarios/user.service';
import { UserProfileDto } from '../../proxy/usuarios/models';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-user-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './user-search.html',
  styleUrls: ['./user-search.scss']
})
export class UserSearchComponent implements OnInit {
  private userService = inject(UserService);
  
  searchQuery: string = '';
  searchSubject: Subject<string> = new Subject<string>();
  
  users: UserProfileDto[] = [];
  loading: boolean = false;
  hasSearched: boolean = false;

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(query => {
      this.performSearch(query);
    });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchQuery);
  }

  performSearch(query: string): void {
    if (!query || query.trim() === '') {
      this.users = [];
      this.hasSearched = false;
      return;
    }

    this.loading = true;
    this.hasSearched = true;
    
    this.userService.searchUsers(query).subscribe({
      next: (results) => {
        this.users = results;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al buscar usuarios', err);
        this.loading = false;
      }
    });
  }

  getAvatarInitials(user: UserProfileDto): string {
    const name = user.nombre || user.userName || 'U';
    return name.charAt(0).toUpperCase();
  }
}
