import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {

  public currentMatch: CurrentMatchResult;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getCurrentMatch();
  }

  getCurrentMatch(): void {
    this.http.get<CurrentMatchResult>(this.baseUrl + 'match/current').subscribe(result => {
      this.currentMatch = result;
      console.log(result);

    }, error => console.error(error));
  }

  play(): void {
    this.http.post<CurrentMatchResult>(this.baseUrl + 'match/play', { }).subscribe(result => {
      this.currentMatch = result;
    }, error => console.error(error));
  }

  onRefresh(): void {
    this.getCurrentMatch();
  }
}


interface CurrentMatchResult {
  matchId: number;
  matchName: string;
  expiryDate: Date;
  yourNumber: string;
  alreadyPlayed: boolean;
}
