export interface UserIdentity {
  id: number;
  userId: string;
  fullName: string;
  email: string;
  sourceSystem: string;
  lastUpdated: string;
  isActive: boolean;
}

export interface UserIdentityUpdate {
  fullName?: string;
  email?: string;
  isActive?: boolean;
}
