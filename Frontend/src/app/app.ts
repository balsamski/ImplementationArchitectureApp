// @ts-nocheck
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected fileName = '';
  protected selectedFile: File | null = null;
  protected statusMessage = '';
  protected isError = false;
  protected isSubmitting = false;

  private readonly backendUrl = '__BACKEND_API_URL__';

  protected onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files && input.files.length > 0 ? input.files[0] : null;
  }

  protected async submit(): Promise<void> {
    if (!this.fileName.trim()) {
      this.setError('Podaj nazwę pliku.');
      return;
    }

    if (!this.selectedFile) {
      this.setError('Wybierz plik do wysłania.');
      return;
    }

    this.isSubmitting = true;
    this.statusMessage = '';
    this.isError = false;
    let timeoutId: number | undefined;

    try {
      const base64Content = await this.toBase64(this.selectedFile);
      const controller = new AbortController();
      timeoutId = window.setTimeout(() => controller.abort(), 15000);

      const response = await fetch(this.backendUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        signal: controller.signal,
        body: JSON.stringify({
          fileName: this.fileName,
          base64Content
        })
      });

      if (!response.ok) {
        const message = await response.text();
        this.setError(message || `Nie udało się wysłać pliku (HTTP ${response.status}).`);
        return;
      }

      this.setSuccess('Sukces: plik został wysłany.');
      this.fileName = '';
      this.selectedFile = null;
    } catch (error) {
      if (error instanceof DOMException && error.name === 'AbortError') {
        this.setError('Przekroczono czas oczekiwania na odpowiedź serwera.');
        return;
      }

      const message = error instanceof Error ? error.message : 'Wystąpił błąd podczas wysyłania pliku.';
      this.setError(message);
    } finally {
      if (timeoutId) {
        window.clearTimeout(timeoutId);
      }

      this.isSubmitting = false;
    }
  }

  private setSuccess(message: string): void {
    this.isError = false;
    this.statusMessage = message;
  }

  private setError(message: string): void {
    this.isError = true;
    this.statusMessage = message;
  }

  private toBase64(file: File): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        const result = reader.result;
        if (typeof result !== 'string') {
          reject(new Error('Nie udało się odczytać pliku.'));
          return;
        }

        const commaIndex = result.indexOf(',');
        resolve(commaIndex >= 0 ? result.slice(commaIndex + 1) : result);
      };

      reader.onerror = () => reject(reader.error ?? new Error('Nie udało się odczytać pliku.'));
      reader.readAsDataURL(file);
    });
  }
}
