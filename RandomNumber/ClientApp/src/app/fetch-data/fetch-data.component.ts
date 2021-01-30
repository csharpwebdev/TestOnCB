import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public matches: MatchResult[];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getPastMatches();
  }
   
  getPastMatches(): void {
    this.http.get<MatchResult[]>(this.baseUrl + 'match/getpast').subscribe(result => {
      this.matches = result;
    }, error => console.error(error));
  }

  onRefresh(): void {
    this.getPastMatches();
  }
}

interface MatchResult {
  matchName: string;
  winnerName: string;
  winnerValue: string;
}
