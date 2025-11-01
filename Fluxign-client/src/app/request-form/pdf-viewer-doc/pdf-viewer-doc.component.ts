import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as pdfjsLib from 'pdfjs-dist';
import worker from 'pdfjs-dist/build/pdf.worker.entry';

@Component({
  selector: 'app-pdf-viewer-doc',
  templateUrl: './pdf-viewer-doc.component.html',
  styleUrls: ['./pdf-viewer-doc.component.scss']
})
export class PdfViewerDocComponent implements OnInit, OnChanges {
  @ViewChild('pdfContainer', { static: true }) pdfContainer!: ElementRef<HTMLDivElement>;
  @Output() positionSelected = new EventEmitter<{ x: number, y: number, page: number; index: number }>();
  @Input() pdfBase64 = '';
  isLoading = false;
  @Input() signerPositions: { x: number, y: number, page: number, name: string }[] = [];

  constructor(private route: ActivatedRoute) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = worker;
  }

  ngOnInit(): void {
    this.isLoading = true;
    // TODO: Set pdfBase64 or accept as Input()
    if (this.pdfBase64) {
      setTimeout(() => {
        this.isLoading = true;
        this.renderPdfFromBase64(this.pdfBase64);
      }, 1000);
    }
  }

  async renderPdfFromBase64(base64: string) {
    if (base64.startsWith('data:')) {
      base64 = base64.split(',')[1];
    }
    base64 = base64.replace(/\s/g, '');
    base64 = base64.replace(/-/g, '+').replace(/_/g, '/');

    try {
      const blob = this.base64ToBlob(base64, 'application/pdf');
      await this.renderAllPages(blob);
    } catch (e) {
      console.error('Error rendering PDF:', e);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    const requestId = this.route.snapshot.queryParamMap.get('reqId');
    if (changes['pdfBase64'] && changes['pdfBase64'].currentValue && requestId != null) {
      this.isLoading = true;
      setTimeout(() => {
        this.renderPdfFromBase64(this.pdfBase64);
      }, 1000);
    }

    if (changes['signerPositions'] && !changes['signerPositions'].firstChange) {
      // Only rerender positions if PDF is already loaded
      const container = this.pdfContainer?.nativeElement;
      if (container && container.children.length) {
        const pageWrappers = Array.from(container.children).filter(c => c.tagName === 'DIV') as HTMLElement[];
        this.renderSignerPositions(pageWrappers, container.clientWidth);
      }
    }
  }

  async renderAllPages(blob: Blob) {
    const url = URL.createObjectURL(blob);
    const loadingTask = pdfjsLib.getDocument(url);
    const pdf = await loadingTask.promise;

    const devicePixelRatio = window.devicePixelRatio || 1;
    const container = this.pdfContainer.nativeElement;

    container.innerHTML = '';
    container.style.position = 'relative';
    container.style.userSelect = 'none';

    const containerWidth = container.clientWidth;
    const pageWrappers: HTMLElement[] = [];

    for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
      const page = await pdf.getPage(pageNum);
      const unscaledViewport = page.getViewport({ scale: 1 });
      const scale = containerWidth / unscaledViewport.width;
      const viewport = page.getViewport({ scale });

      const canvas = document.createElement('canvas');
      const context = canvas.getContext('2d')!;

      canvas.width = viewport.width * devicePixelRatio;
      canvas.height = viewport.height * devicePixelRatio;
      canvas.style.width = `${viewport.width}px`;
      canvas.style.height = `${viewport.height}px`;

      context.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, 0, 0);
      await page.render({ canvasContext: context, viewport }).promise;

      const pageWrapper = document.createElement('div');
      pageWrapper.style.position = 'relative';
      pageWrapper.style.width = `${viewport.width}px`;
      pageWrapper.style.height = `${viewport.height}px`;
      pageWrapper.style.marginBottom = '16px';

      pageWrapper.appendChild(canvas);
      container.appendChild(pageWrapper);

      pageWrappers.push(pageWrapper);
    }

    // Now render draggable signer rectangles
    this.renderSignerPositions(pageWrappers, containerWidth);

    this.isLoading = false;
  }


  base64ToBlob(base64: string, contentType = 'application/pdf'): Blob {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: contentType });
  }

  private renderSignerPositions(pageWrappers: HTMLElement[], containerWidth: number) {
    const container = this.pdfContainer.nativeElement;
    container.querySelectorAll('.signer-marker').forEach(m => m.remove());

    let activeMarker: HTMLElement | null = null;
    let activeIndex: number | null = null;
    let isFloating = false;

    // Move active marker with cursor anywhere on screen
    const onMouseMove = (e: MouseEvent) => {
      if (!activeMarker || !isFloating) return;

      // Position marker centered on cursor with fixed positioning
      const newLeft = e.clientX - activeMarker.clientWidth / 2;
      const newTop = e.clientY - activeMarker.clientHeight / 2;

      activeMarker.style.position = 'fixed';
      activeMarker.style.left = `${newLeft}px`;
      activeMarker.style.top = `${newTop}px`;
      activeMarker.style.zIndex = '9999';
      activeMarker.style.pointerEvents = 'none'; // Prevent interference with clicks
    };

    // On second click anywhere, drop marker inside container and update position
    const onSecondClick = (e: MouseEvent) => {
      if (!activeMarker || activeIndex === null || !isFloating) return;

      e.preventDefault();
      e.stopPropagation();

      // Remove event listeners
      document.removeEventListener('mousemove', onMouseMove);
      document.removeEventListener('click', onSecondClick);
      document.removeEventListener('keydown', onKeyDown);

      // Reset cursor and marker properties
      document.body.style.cursor = '';
      activeMarker.style.pointerEvents = '';
      isFloating = false;

      // Get container bounds
      const containerRect = container.getBoundingClientRect();

      // Calculate position relative to container (accounting for scroll)
      let dropLeft = e.clientX - containerRect.left - activeMarker.clientWidth / 2;
      let dropTop = e.clientY - containerRect.top - activeMarker.clientHeight / 2 + container.scrollTop;

      // Clamp position inside container bounds
      dropLeft = Math.max(0, Math.min(dropLeft, container.clientWidth - activeMarker.clientWidth));
      dropTop = Math.max(0, Math.min(dropTop, container.scrollHeight - activeMarker.clientHeight));

      // Change marker to absolute positioning within container
      activeMarker.style.position = 'absolute';
      activeMarker.style.left = `${dropLeft}px`;
      activeMarker.style.top = `${dropTop}px`;
      activeMarker.style.zIndex = '10';

      // Find which page the marker center is over
      let foundPageIndex = 0;
      const markerCenterY = dropTop + activeMarker.clientHeight / 2;

      for (let i = 0; i < pageWrappers.length; i++) {
        const pw = pageWrappers[i];
        const pwTop = pw.offsetTop;
        const pwBottom = pwTop + pw.clientHeight;

        if (markerCenterY >= pwTop && markerCenterY < pwBottom) {
          foundPageIndex = i;
          break;
        }
      }

      // Calculate PDF coordinates
      const targetPageWrapper = pageWrappers[foundPageIndex];
      const targetCanvas = targetPageWrapper.querySelector('canvas')!;
      const targetScale = targetCanvas.clientWidth / targetCanvas.width;

      // Position relative to the page wrapper
      const relativeX = dropLeft - targetPageWrapper.offsetLeft;
      const relativeY = dropTop - targetPageWrapper.offsetTop;

      // Convert to PDF coordinates
      const pdfX = relativeX / targetScale;
      const pdfY = (targetCanvas.height - (relativeY + activeMarker.clientHeight) / targetScale);

      // Update position data
      this.signerPositions[activeIndex] = {
        ...this.signerPositions[activeIndex],
        x: pdfX,
        y: pdfY,
        page: foundPageIndex + 1,
      };

      // Emit the new position
      this.positionSelected.emit({
        x: pdfX,
        y: pdfY,
        page: foundPageIndex + 1,
        index: activeIndex,
      });

      // Reset active states
      activeMarker = null;
      activeIndex = null;
    };

    // Render existing signer positions
    this.signerPositions.forEach((pos, index) => {
      const pageWrapper = pageWrappers[pos.page - 1];
      if (!pageWrapper) return;

      const canvas = pageWrapper.querySelector('canvas')!;
      const scale = canvas.clientWidth / canvas.width;

      const rectWidth = containerWidth * 0.32;
      const rectHeight = pageWrapper.clientHeight * 0.15;

      // Calculate position on the page
      const left = pageWrapper.offsetLeft + pos.x * scale;
      const top = pageWrapper.offsetTop + (canvas.height * scale - (pos.y * scale + rectHeight));

      // Create marker element
      const marker = document.createElement('div');
      marker.classList.add('signer-marker');
      marker.setAttribute('data-index', index.toString());

      marker.style.height = `${rectHeight * 0.6}px`;

      // Other styles remain unchanged
      marker.style.position = 'absolute';
      marker.style.left = `${left}px`;
      marker.style.top = `${top}px`;
      marker.style.width = `${rectWidth}px`;
      marker.style.backgroundColor = 'rgba(0, 0, 0, 0)';
      marker.style.border = '2px solid #6bc91f';
      marker.style.borderRadius = '6px';
      marker.style.boxSizing = 'border-box';
      marker.style.cursor = 'grab';
      marker.style.zIndex = '10';
      marker.style.display = 'flex';
      marker.style.alignItems = 'center';
      marker.style.justifyContent = 'center';
      marker.style.color = 'darkred';
      marker.style.fontWeight = 'bold';
      marker.style.userSelect = 'none';
      marker.style.fontSize = '12px';

      // Clear previous content
      marker.innerHTML = '';

      // Logo container - fixed width, full height
      const logoContainer = document.createElement('div');
      logoContainer.style.flex = '0 0 40px';
      logoContainer.style.height = '100%';

      // Logo image fills entire logo container
      const logo = document.createElement('img');
      logo.src = 'assets/images/favicon-logo.png';
      logo.alt = 'Logo';
      logo.style.width = '100%';
      logo.style.height = '100%';
      logo.style.objectFit = 'contain';

      logoContainer.appendChild(logo);

      // Text container - fill remaining space
      const textContainer = document.createElement('div');
      textContainer.style.flex = '1';
      textContainer.style.display = 'flex';
      textContainer.style.flexDirection = 'column';
      textContainer.style.justifyContent = 'center';
      textContainer.style.marginLeft = '8px';

      // Name line (bold, green)
      const nameDiv = document.createElement('div');
      nameDiv.textContent = pos.name;
      nameDiv.style.fontWeight = 'bold';
      nameDiv.style.color = '#555';
      nameDiv.style.fontSize = '11px';

      // Details line (gray)
      const detailsDiv = document.createElement('div');
      detailsDiv.textContent = 'Signer Details here';  // replace with actual details
      detailsDiv.style.fontSize = '8px';
      detailsDiv.style.color = '#555';

      // Date line (gray)
      const dateDiv = document.createElement('div');
      const today = new Date();
      const yyyy = today.getFullYear();
      const mm = String(today.getMonth() + 1).padStart(2, '0'); // Months start at 0
      const dd = String(today.getDate()).padStart(2, '0');
      const formattedDate = `${yyyy}-${mm}-${dd}`;
      
      dateDiv.textContent = `Signed Date: ${formattedDate}`;
      
      dateDiv.style.fontSize = '8px';
      dateDiv.style.color = '#555';

      // Append text lines
      textContainer.appendChild(nameDiv);
      textContainer.appendChild(detailsDiv);
      textContainer.appendChild(dateDiv);

      // Append logo and text containers to marker
      marker.appendChild(logoContainer);
      marker.appendChild(textContainer);

      // Handle first click - start floating
      marker.addEventListener('click', (e: MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();

        // Only allow one marker to be active at a time
        if (activeMarker || isFloating) return;

        activeMarker = marker;
        activeIndex = index;
        isFloating = true;

        // Change cursor to indicate floating mode
        document.body.style.cursor = 'crosshair';

        // Start following mouse immediately
        onMouseMove(e);

        // Add global event listeners for mouse movement and second click
        document.addEventListener('mousemove', onMouseMove);

        // Use setTimeout to prevent the same click from being treated as second click
        setTimeout(() => {
          document.addEventListener('click', onSecondClick);
        }, 50);
      });

      // Prevent default drag behavior
      marker.addEventListener('dragstart', (e) => {
        e.preventDefault();
      });

      container.appendChild(marker);
    });

    // Handle clicks outside markers to cancel floating if needed
    const cancelFloating = (e: MouseEvent) => {
      if (!isFloating || !activeMarker) return;

      // Check if click is on a marker
      const target = e.target as HTMLElement;
      if (target && target.classList.contains('signer-marker')) return;

      // If ESC key functionality is desired, you can add it here
    };

    // Optional: Add ESC key to cancel floating
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isFloating && activeMarker) {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('click', onSecondClick);
        document.removeEventListener('keydown', onKeyDown);

        document.body.style.cursor = '';
        activeMarker.style.pointerEvents = '';

        // Reset marker to original position (re-render)
        this.renderSignerPositions(pageWrappers, containerWidth);

        activeMarker = null;
        activeIndex = null;
        isFloating = false;
      }
    };

    // Add keydown listener when function starts
    document.addEventListener('keydown', onKeyDown);
  }
}  