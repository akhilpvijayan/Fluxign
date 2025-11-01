import { Component, OnInit } from '@angular/core';
import { FileService } from './services/file.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title="EKYC"
  fileContent: string | null = null;
  isDark = false;
  constructor(private translate: TranslateService) {
    const lang = localStorage.getItem('lang') || 'en';
    translate.setDefaultLang(lang);
    translate.use(lang);
  }
  

  ngOnInit() {
    // Initialize dark mode based on saved preference or system settings
    const saved = localStorage.getItem('darkMode');
    if (saved === 'true' || (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
      this.isDark = true;
      document.documentElement.classList.add('dark');
    }
  }
  
  toggleDarkMode() {
    this.isDark = !this.isDark;
    if (this.isDark) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('darkMode', 'true'); // optional: save preference
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('darkMode', 'false');
    }
  }
}
