import { GridModule } from '@syncfusion/ej2-angular-grids'
import { PageService, SortService, FilterService, GroupService } from '@syncfusion/ej2-angular-grids'
import { Component, OnInit } from '@angular/core';
import { data } from './datasource';
import { PageSettingsModel } from '@syncfusion/ej2-angular-grids';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [PageService,
    SortService,
    FilterService,
    GroupService],
})


export class AppComponent implements OnInit {

  public data?: object[];
  public pageSettings?: PageSettingsModel;

  ngOnInit(): void {
    this.data = data;
    this.pageSettings = { pageSize: 6 };
  }
}
