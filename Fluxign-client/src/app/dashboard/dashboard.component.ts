import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { RequestDashboard } from '../models/request-dashboard';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  activeTab: string = 'documents';
  underlineLeft: number = 0;
  underlineWidth: number = 0;
  isProfileMenuOpen = false;
  showLogoutModal = false;
  user: any;

  @ViewChild('tabDocuments') tabDocumentsRef!: ElementRef;
  @ViewChild('tabSettings') tabSettingsRef!: ElementRef;
  @ViewChild('underline') underlineRef!: ElementRef;
 
constructor(private router: Router, private authService: AuthService, private userService: UserService){}

  ngOnInit(): void {
    this.userService.getProfile().subscribe((res: any)=>{
      this.user = res.data;
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.updateUnderline());
  }
  
  setActiveTab(tab: string): void {
    this.activeTab = tab;
    this.updateUnderline();
  }

  updateUnderline(): void {
    let targetRef: ElementRef;

    switch (this.activeTab) {
      case 'documents':
        targetRef = this.tabDocumentsRef;
        break;
      case 'settings':
        targetRef = this.tabSettingsRef;
        break;
      default:
        return;
    }

    const element = targetRef.nativeElement as HTMLElement;
    this.underlineLeft = element.offsetLeft;
    this.underlineWidth = element.offsetWidth;
  }

  toggleProfileMenu() {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }

  @HostListener('document:click')
  onDocumentClick() {
    if (this.isProfileMenuOpen) {
      this.isProfileMenuOpen = false;
    }
  }

  confirmLogout(event: Event) {
    event.preventDefault();
    const confirmed = confirm('Are you sure you want to logout?');
    if (confirmed) {
      this.logout();
    }
  }

  logout() {
    this.showLogoutModal = false;
    this.authService.logout();
  this.router.navigate(['/signin']);
  }

  onVerifyEmail(){
    
  }
}