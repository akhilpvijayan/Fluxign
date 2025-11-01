import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-settings-tab',
  templateUrl: './settings-tab.component.html',
  styleUrls: ['./settings-tab.component.scss']
})
export class SettingsTabComponent implements OnInit {
  isDark: boolean = false;
  settingsSection: string = 'general';
  @ViewChild('generalNav', { static: false }) generalNavRef!: ElementRef;
  @ViewChild('appearanceNav', { static: false }) appearanceNavRef!: ElementRef;
  @ViewChild('settingsUnderline', { static: false }) settingsUnderlineRef!: ElementRef;
  @ViewChild('languageNav', { static: false }) languageNavRef!: ElementRef;

  @ViewChild('languageDropdown') languageDropdownRef!: ElementRef;

  settingsUnderlineStyles = {};
  dropdownOpen = false;
  selectedLanguage = 'en';

  constructor(private translate: TranslateService) { 
    const savedLang = localStorage.getItem('lang') || 'en';
    this.setLanguage(savedLang);
  } 

  ngOnInit(): void {
    const saved = localStorage.getItem('darkMode');
    this.isDark = saved === 'true' || (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches);
    if (this.isDark) document.documentElement.classList.add('dark');
  
    const savedLang = localStorage.getItem('lang') || 'en';
    this.setLanguage(savedLang);
  }
  
  
  setLanguage(lang: string) {
    this.selectedLanguage = lang;
    this.translate.use(lang); // This actually loads the JSON file
    localStorage.setItem('lang', lang);
    document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;
  }
  

  setSettingsSection(section: string) {
    this.settingsSection = section;
  }

  toggleDarkMode(value: boolean) {
    this.isDark = value;
    if (value) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('darkMode', 'true');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('darkMode', 'false');
    }
  }

  updateSettingsUnderline() {
    let targetRef: ElementRef;

    switch (this.settingsSection) {
      case 'general':
        targetRef = this.generalNavRef;
        break;
      case 'appearance':
        targetRef = this.appearanceNavRef;
        break;
      case 'language':
        targetRef = this.languageNavRef;
        break;
      default:
        return;
    }

    const el = targetRef.nativeElement as HTMLElement;
    const parent = el.parentElement!.parentElement!;

    if (window.innerWidth >= 768) {
      // For md+ screens vertical nav - underline is vertical bar on left side of the button
      this.settingsUnderlineStyles = {
        top: el.offsetTop + 'px',
        height: el.offsetHeight + 'px',
        left: '0',
        width: '4px',
        bottom: 'auto',
        right: 'auto',
      };
    } else {
      // For small screens horizontal nav - underline is bottom border
      this.settingsUnderlineStyles = {
        left: el.offsetLeft + 'px',
        width: el.offsetWidth + 'px',
        bottom: '0',
        top: 'auto',
        height: '4px',
        right: 'auto',
      };
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.updateSettingsUnderline();
    });

    window.addEventListener('resize', () => {
      this.updateSettingsUnderline();
    });
  }

  selectLanguage(lang: string) {
    this.selectedLanguage = lang;
    this.dropdownOpen = false;
    this.applyLanguageChange(lang);
  }
  
  onLanguageChange(event: Event) {
    const newLang = (event.target as HTMLSelectElement).value;
    this.applyLanguageChange(newLang);
  }
  
  private applyLanguageChange(lang: string) {
    this.selectedLanguage = lang;
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    const html = document.documentElement;
    html.lang = lang;
    html.dir = lang === 'ar' ? 'rtl' : 'ltr';
  }  

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (this.languageDropdownRef && !this.languageDropdownRef.nativeElement.contains(target)) {
      this.dropdownOpen = false;
    }
  }
}
