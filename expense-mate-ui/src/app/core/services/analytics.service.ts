import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AnalyticsSummaryDto } from '../models/analytics.model';


@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {

  private http = inject(HttpClient);

  private apiUrl =
    'http://localhost:5001/api/Analytics';


  getAnalyticsSummary(
    year?: number
  ): Observable<AnalyticsSummaryDto> {

    let url =
      `${this.apiUrl}/summary`;


    if (
      year !== undefined &&
      year !== null
    ) {

      url += `?year=${year}`;

    }


    return this.http.get<AnalyticsSummaryDto>(
      url
    );
  }
}
