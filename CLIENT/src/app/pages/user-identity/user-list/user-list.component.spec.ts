import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserListComponent } from './user-list.component';
import { UserIdentity } from '../../../shared/models/user-identity.model';
import { BusyService } from '../../../shared/services/busy.service';
import { Pagination } from '../../../shared/models/pagination';
import { signal } from '@angular/core';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let busyServiceSpy: jasmine.SpyObj<BusyService>;

  // Mock user data
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

  const mockPagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 25,
    totalPages: 3
  };

  beforeEach(async () => {
    // Create a real BusyService with actual signals
    busyServiceSpy = {
      busyRequestCount: 0,
      isLoading: signal(false),
      busy: jasmine.createSpy('busy'),
      idle: jasmine.createSpy('idle')
    } as any;
    
    await TestBed.configureTestingModule({
      imports: [UserListComponent],
      providers: [
        { provide: BusyService, useValue: busyServiceSpy }
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit userSelected event when selectUser is called', () => {
    // Setup
    component.userIdentities = mockUsers;
    fixture.detectChanges();
    
    // Spy on the output event
    spyOn(component.userSelected, 'emit');
    
    // Call the method
    component.selectUser(mockUsers[0]);
    
    // Verify
    expect(component.userSelected.emit).toHaveBeenCalledWith(mockUsers[0]);
  });

  it('should track users by id, page, and index', () => {
    // Setup
    component.userIdentities = mockUsers;
    component.pagination = mockPagination;
    fixture.detectChanges();
    
    // Call the method
    const trackId1 = component.trackByUser(mockUsers[0], 0);
    const trackId2 = component.trackByUser(mockUsers[1], 1);
    
    // Verify
    expect(trackId1).toBe(`page1-index0-id1`);
    expect(trackId2).toBe(`page1-index1-id2`);
  });

  it('should handle tracking when pagination is null', () => {
    // Setup
    component.userIdentities = mockUsers;
    component.pagination = null;
    fixture.detectChanges();
    
    // Call the method
    const trackId = component.trackByUser(mockUsers[0], 0);
    
    // Verify
    expect(trackId).toBe(`page1-index0-id1`);
  });

  it('should handle tracking when index is not provided', () => {
    // Setup
    component.userIdentities = mockUsers;
    fixture.detectChanges();
    
    // Call the method
    const trackId = component.trackByUser(mockUsers[0]);
    
    // Verify
    expect(trackId).toBe(`page1-index0-id1`);
  });
});
