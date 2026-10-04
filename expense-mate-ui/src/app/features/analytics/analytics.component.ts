import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AnalyticsService } from '../../core/services/analytics.service';
import { AnalyticsSummaryDto } from '../../core/models/analytics.model';

@Component({
  selector: 'app-analytics',
  standalone: true,

  imports: [
    CommonModule,
    FormsModule
  ],

  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.css'
})
export class AnalyticsComponent implements OnInit {

  private analyticsService = inject(AnalyticsService);


  // ============================
  // ANALYTICS DATA
  // ============================

  data = signal<AnalyticsSummaryDto | null>(null);


  // ============================
  // LOADING
  // ============================

  isLoading = signal<boolean>(false);


  // ============================
  // ERROR
  // ============================

  errorMessage = signal<string | null>(null);


  // ============================
  // YEAR
  // ============================

  selectedYear = signal<number>(
    new Date().getFullYear()
  );


  years: number[] = [
    2026,
    2025,
    2024,
    2023
  ];


  // ============================
  // MAXIMUM CHART VALUE
  // ============================

  maxAmount = signal<number>(100);


  // ============================
  // INITIALIZE
  // ============================

  ngOnInit(): void {

    this.loadAnalytics();

  }


  // ============================
  // LOAD ANALYTICS
  // ============================

  loadAnalytics(): void {

    this.isLoading.set(true);

    this.errorMessage.set(null);


    const year =
      this.selectedYear();


    console.log(
      'Loading analytics for year:',
      year
    );


    this.analyticsService
      .getAnalyticsSummary(year)
      .subscribe({

        next: (response: AnalyticsSummaryDto) => {

          console.log(
            'Analytics response:',
            response
          );


          // Store API response
          this.data.set(response);


          // Calculate chart maximum
          this.calculateMaxAmount();


          // Stop loading
          this.isLoading.set(false);


          console.log(
            'Analytics data stored successfully'
          );

        },


        error: (error) => {

          console.error(
            'Analytics error:',
            error
          );


          this.isLoading.set(false);


          this.errorMessage.set(
            error?.error?.message ||
            `Failed to fetch analytics (HTTP ${error?.status || 'Error'}).`
          );

        }

      });

  }


  // ============================
  // YEAR CHANGE
  // ============================

  onYearChange(): void {

    this.loadAnalytics();

  }


  // ============================
  // CALCULATE MAXIMUM
  // ============================

  calculateMaxAmount(): void {

    const analytics =
      this.data();


    if (
      !analytics ||
      !analytics.monthlyOverview ||
      analytics.monthlyOverview.length === 0
    ) {

      this.maxAmount.set(100);

      return;

    }


    const incomeAmounts =
      analytics.monthlyOverview.map(
        month => Number(month.totalIncome) || 0
      );


    const expenseAmounts =
      analytics.monthlyOverview.map(
        month => Number(month.totalExpense) || 0
      );


    const maxIncome =
      Math.max(...incomeAmounts);


    const maxExpense =
      Math.max(...expenseAmounts);


    const maximum =
      Math.max(
        maxIncome,
        maxExpense
      );


    this.maxAmount.set(
      maximum > 0
        ? maximum
        : 100
    );

  }


  // ============================
  // BAR HEIGHT
  // ============================

  getBarHeight(
    amount: number
  ): number {

    if (
      !amount ||
      amount <= 0
    ) {

      return 0;

    }


    const maximum =
      this.maxAmount();


    if (maximum <= 0) {

      return 0;

    }


    return Math.min(
      Math.round(
        (amount / maximum) * 100
      ),
      100
    );

  }

}
