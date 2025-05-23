import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserIdentityService } from './user-identity.service';
import { UserIdentity, UserIdentityUpdate } from '../models/user-identity.model';
import { environment } from '../../../environments/environment';
import { HttpResponse } from '@angular/common/http';
import { UserIdentityParams } from '../models/user-identity-params';

describe('UserIdentityService', () => {
  let service: UserIdentityService;
  let httpMock: HttpTestingController;
  const baseUrl = environment.apiUrl;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserIdentityService]
    });
    
    service = TestBed.inject(UserIdentityService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Verifies that no requests are outstanding
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve a user identity by id', () => {
    const mockUser: UserIdentity = {
      id: 1,
      userId: 'user-123',
      fullName: 'Test User',
      email: 'test@example.com',
      sourceSystem: 'local',
      lastUpdated: new Date().toISOString(),
      isActive: true
    };

    service.getUserIdentity(1).subscribe(user => {
      expect(user).toEqual(mockUser);
    });

    const req = httpMock.expectOne(`${baseUrl}identities/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

  it('should retrieve all user identities', () => {
    const mockUsers: UserIdentity[] = [
      {
        id: 1,
        userId: 'user-1',
        fullName: 'User One',
        email: 'user1@example.com',
        sourceSystem: 'local',
        lastUpdated: new Date().toISOString(),
        isActive: true
      },
      {
        id: 2,
        userId: 'user-2',
        fullName: 'User Two',
        email: 'user2@example.com',
        sourceSystem: 'local',
        lastUpdated: new Date().toISOString(),
        isActive: true
      }
    ];

    service.getAllUserIdentities().subscribe(users => {
      expect(users.length).toBe(2);
      expect(users).toEqual(mockUsers);
    });

    const req = httpMock.expectOne(`${baseUrl}identities`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUsers);
  });

  it('should update a user identity', () => {
    const mockId = 1;
    const mockUpdate: UserIdentityUpdate = {
      fullName: 'Updated User',
      email: 'updated@example.com',
      isActive: true
    };
    
    const mockUpdatedUser: UserIdentity = {
      id: mockId,
      userId: 'user-123',
      fullName: mockUpdate.fullName!,
      email: mockUpdate.email!,
      sourceSystem: 'local',
      lastUpdated: new Date().toISOString(),
      isActive: mockUpdate.isActive!
    };

    service.updateUserIdentity(mockId, mockUpdate).subscribe(user => {
      expect(user).toEqual(mockUpdatedUser);
    });

    const req = httpMock.expectOne(`${baseUrl}identities/${mockId}`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(mockUpdate);
    req.flush(mockUpdatedUser);
  });

  it('should set and get user identity params', () => {
    const params: Partial<UserIdentityParams> = {
      pageNumber: 2,
      pageSize: 10,
      searchString: 'test'
    };
    
    service.setUserIdentityParams(params);
    const retrievedParams = service.getUserIdentityParams();
    
    expect(retrievedParams.pageNumber).toBe(2); // Use the actual value instead of params.pageNumber
    expect(retrievedParams.pageSize).toBe(10); // Use the actual value instead of params.pageSize
    expect(retrievedParams.searchString).toBe('test'); // Use the actual value instead of params.searchString
  });

  it('should reset user identity params', () => {
    // First set some params
    service.setUserIdentityParams({
      pageNumber: 2,
      pageSize: 10,
      searchString: 'test'
    });
    
    // Then reset them
    service.resetUserIdentityParams();
    const resetParams = service.getUserIdentityParams();
    
    // Check if they are reset to default values
    expect(resetParams.pageNumber).toBe(1); // Assuming 1 is the default
    expect(resetParams.pageSize).toBe(5);  // Assuming 5 is the default
    expect(resetParams.searchString).toBeNull(); // Actual default is null
  });
});
