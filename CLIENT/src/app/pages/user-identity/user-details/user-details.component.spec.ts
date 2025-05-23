import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserDetailsComponent } from './user-details.component';
import { UserIdentity } from '../../../shared/models/user-identity.model';
import { DatePipe } from '@angular/common';
import { BusyService } from '../../../shared/services/busy.service';
import { signal } from '@angular/core';

describe('UserDetailsComponent', () => {
  let component: UserDetailsComponent;
  let fixture: ComponentFixture<UserDetailsComponent>;
  
  // Mock user data
  const mockUser: UserIdentity = {
    id: 1,
    userId: 'user-123',
    fullName: 'Test User',
    email: 'test@example.com',
    sourceSystem: 'local',
    lastUpdated: new Date().toISOString(),
    isActive: true
  };

  beforeEach(async () => {
    // Create a real BusyService with actual signals
    const busyServiceMock = {
      busyRequestCount: 0,
      isLoading: signal(false),
      busy: jasmine.createSpy('busy'),
      idle: jasmine.createSpy('idle')
    } as any;
    
    await TestBed.configureTestingModule({
      imports: [UserDetailsComponent, DatePipe],
      providers: [
        { provide: BusyService, useValue: busyServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDetailsComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should display user details when user input is provided', () => {
    // Set the input property
    component.user = mockUser;
    fixture.detectChanges();
    
    // Get the component element
    const compiled = fixture.nativeElement;
    
    // Check that user details are displayed
    expect(compiled.textContent).toContain(mockUser.fullName);
    expect(compiled.textContent).toContain(mockUser.email);
    expect(compiled.textContent).toContain(mockUser.userId);
  });

  it('should emit editRequested event when requestEdit is called', () => {
    // Spy on the output event
    spyOn(component.editRequested, 'emit');
    
    // Call the method that should emit the event
    component.requestEdit();
    
    // Verify the event was emitted
    expect(component.editRequested.emit).toHaveBeenCalled();
  });
});
