
export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

export interface LoginUserDto {
  userNameOrEmail: string;
  password: string;
}

export interface RegisterUserDto {
  userName: string;
  email: string;
  password: string;
}

export interface UpdateUserProfileDto {
  nombre: string;
  apellido: string;
  userName?: string;
  email?: string;
  telefono?: string;
  fotoUrl?: string;
}

export interface UserProfileDto {
  id?: string;
  userName?: string;
  email?: string;
  nombre?: string;
  apellido?: string;
  telefono?: string;
  fotoUrl?: string;
}
