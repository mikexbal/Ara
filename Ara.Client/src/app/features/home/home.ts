import { Component, DestroyRef, HostListener, inject, signal } from '@angular/core';

import { Destination } from '../../core/models/destination';
import { DestinationService } from '../../core/services/destination.service';
import { ImageService } from '../../core/services/image.service';
import { UserMenu } from '../../shared/user-menu/user-menu';

const HERO_IMAGE_ROTATION_MS = 7000;

type LayerUrls = [string | null, string | null];

@Component({
  selector: 'app-home',
  imports: [UserMenu],
  templateUrl: './home.html'
})
export class Home {
  private readonly destinationService = inject(DestinationService);
  private readonly imageService = inject(ImageService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly featuredIslands = signal<Destination[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly scrolled = signal(false);

  private readonly heroImages = signal<string[]>([]);
  private readonly currentImageIndex = signal(0);

  // Only two <img> layers ever exist in the DOM — the visible one and the one
  // being preloaded/faded in behind it — instead of loading every hero photo at once.
  protected readonly layerUrls = signal<LayerUrls>([null, null]);
  protected readonly frontLayer = signal<0 | 1>(0);

  @HostListener('window:scroll')
  onWindowScroll(): void {
    this.scrolled.set(window.scrollY > 16);
  }

  constructor() {
    this.destinationService.getAll().subscribe({
      next: (destinations) => {
        this.featuredIslands.set(destinations.slice(0, 4));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load featured islands. Is the API running?');
        this.loading.set(false);
      }
    });

    this.imageService.getHeroImages().subscribe({
      next: (images) => {
        this.heroImages.set(images);
        if (images.length > 0) {
          this.layerUrls.set([images[0], null]);
        }
      },
      // No configured image bucket yet — the hero keeps its placeholder background.
      error: () => this.heroImages.set([])
    });

    const rotationTimer = setInterval(() => this.advanceHeroImage(), HERO_IMAGE_ROTATION_MS);
    this.destroyRef.onDestroy(() => clearInterval(rotationTimer));
  }

  private async advanceHeroImage(): Promise<void> {
    const images = this.heroImages();
    if (images.length < 2) {
      return;
    }

    const nextIndex = (this.currentImageIndex() + 1) % images.length;
    const backLayer: 0 | 1 = this.frontLayer() === 0 ? 1 : 0;
    const nextUrl = images[nextIndex];

    try {
      await this.preloadImage(nextUrl);
    } catch {
      // Skip a broken image this tick; the next interval will try the one after it.
      this.currentImageIndex.set(nextIndex);
      return;
    }

    this.layerUrls.update((urls) => {
      const updated: LayerUrls = [...urls];
      updated[backLayer] = nextUrl;
      return updated;
    });
    this.frontLayer.set(backLayer);
    this.currentImageIndex.set(nextIndex);
  }

  private preloadImage(url: string): Promise<void> {
    return new Promise((resolve, reject) => {
      const image = new Image();
      image.onload = () => resolve();
      image.onerror = () => reject(new Error(`Failed to load hero image: ${url}`));
      image.src = url;
    });
  }
}
