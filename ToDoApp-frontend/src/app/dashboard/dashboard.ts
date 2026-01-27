import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard {
  kpis = [
    {
      title: 'Total Page Views',
      value: '442,236',
      change: '+59.3%',
      note: 'You made an extra 35,000 this year',
      trend: 'up'
    },
    {
      title: 'Total Users',
      value: '78,250',
      change: '+70.5%',
      note: 'You made an extra 8,900 this year',
      trend: 'up'
    },
    {
      title: 'Total Orders',
      value: '18,800',
      change: '-27.4%',
      note: 'You made an extra 1,943 this year',
      trend: 'down'
    },
    {
      title: 'Total Sales',
      value: '$35,078',
      change: '-27.4%',
      note: 'You made an extra $20,395 this year',
      trend: 'down'
    }
  ];
}
