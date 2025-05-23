import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { UserEditComponent } from './user-edit.component';
import { BusyService } from '../../../shared/services/busy.service';
import { UserIdentity, UserIdentityUpdate } from '../../../shared/models/user-identity.model';
import { signal } from '@angular/core';

describe('UserEditComponent', () => {
  let component: UserEditComponent;
  let fixture: ComponentFixture<UserEditComponent>;
  let busyServiceSpy: jasmine.SpyObj<BusyService>;

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
    busyServiceSpy = {
      busyRequestCount: 0,
      isLoading: signal(false),
      busy: jasmine.createSpy('busy'),
      idle: jasmine.createSpy('idle')
    } as any;
    
    await TestBed.configureTestingModule({
      imports: [UserEditComponent],
      providers: [
        { provide: BusyService, useValue: busyServiceSpy }
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UserEditComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty values when no user is provided', () => {
    component.ngOnInit();
    expect(component.editForm.get('fullName')?.value).toBe('');
    expect(component.editForm.get('email')?.value).toBe('');
    expect(component.editForm.get('isActive')?.value).toBeTrue();
  });

  it('should initialize form with user values when user is provided', () => {
    component.user = mockUser;
    component.ngOnInit();
    
    expect(component.editForm.get('fullName')?.value).toBe(mockUser.fullName);
    expect(component.editForm.get('email')?.value).toBe(mockUser.email);
    expect(component.editForm.get('isActive')?.value).toBe(mockUser.isActive);
  });

  it('should update form when user input changes', () => {
    // Initialize with no user
    component.ngOnInit();
    
    // Then set a user
    component.user = mockUser;
    // Simulate ngOnChanges
    component.ngOnChanges({
      user: {
        currentValue: mockUser,
        previousValue: null,
        firstChange: true,
        isFirstChange: () => true
      }
    });
    
    // Check that form was updated
    expect(component.editForm.get('fullName')?.value).toBe(mockUser.fullName);
    expect(component.editForm.get('email')?.value).toBe(mockUser.email);
  });

  it('should validate required fields', () => {
    component.ngOnInit();
    
    // Set invalid values
    component.editForm.patchValue({
      fullName: '',
      email: 'invalid-email'
    });
    
    // Check validation errors
    const fullNameControl = component.editForm.get('fullName');
    const emailControl = component.editForm.get('email');
    
    expect(fullNameControl?.valid).toBeFalsy();
    expect(fullNameControl?.hasError('required')).toBeTruthy();
    
    expect(emailControl?.valid).toBeFalsy();
    expect(emailControl?.hasError('email')).toBeTruthy();
  });

  it('should emit saveChanges event with updated fields only', () => {
    // Setup
    component.user = mockUser;
    component.ngOnInit();
    
    // Spy on output events
    spyOn(component.saveChanges, 'emit');
    
    // Make changes to only one field
    component.editForm.patchValue({
      fullName: 'Updated Name',
      email: mockUser.email, // Keep the same
      isActive: mockUser.isActive // Keep the same
    });
    
    // Submit the form
    component.submit();
    
    // Verify only changed fields are in the update
    const expectedUpdate: UserIdentityUpdate = {
      fullName: 'Updated Name'
    };
    
    expect(component.saveChanges.emit).toHaveBeenCalledWith(expectedUpdate);
  });

  it('should emit cancelEdit when no changes are made', () => {
    // Setup
    component.user = mockUser;
    component.ngOnInit();
    
    // Spy on output events
    spyOn(component.cancelEdit, 'emit');
    spyOn(component.saveChanges, 'emit');
    
    // Don't make any changes
    
    // Submit the form
    component.submit();
    
    // Verify cancelEdit was called and saveChanges was not
    expect(component.cancelEdit.emit).toHaveBeenCalled();
    expect(component.saveChanges.emit).not.toHaveBeenCalled();
  });

  it('should cancel editing and emit cancelEdit event', () => {
    // Setup
    component.user = mockUser;
    component.ngOnInit();
    
    // Make changes
    component.editForm.patchValue({
      fullName: 'Changed Name'
    });
    
    // Spy on output event
    spyOn(component.cancelEdit, 'emit');
    
    // Call cancel
    component.cancel();
    
    // Verify form was reset and event was emitted
    expect(component.editForm.get('fullName')?.value).toBe(mockUser.fullName);
    expect(component.formSubmitted).toBeFalse();
    expect(component.cancelEdit.emit).toHaveBeenCalled();
  });
});
