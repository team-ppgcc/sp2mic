package com.example.orchestrator.dto;



import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@AllArgsConstructor
@NoArgsConstructor
@Data
@Builder
public class ProgramaCodDTO {

    private String description;
    private String sigla;
    private String code;
}

