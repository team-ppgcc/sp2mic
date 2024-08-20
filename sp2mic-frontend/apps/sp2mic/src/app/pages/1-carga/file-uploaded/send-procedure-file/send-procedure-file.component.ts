import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-send-procedure-file',
  templateUrl: './send-procedure-file.component.html',
  styleUrls: ['./send-procedure-file.component.scss'],
})
export class SendProcedureFileComponent implements OnInit {
  file_upload_config = {
    MIME_types_accepted: '.sql, .SQL',
    is_multiple_selection_allowed: true,
    data: null,
  };

  constructor() {}

  ngOnInit(): void {}
}
