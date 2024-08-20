package com.example.orchestrator.controller;

import org.springframework.web.bind.annotation.RestController;
import java.util.List;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.GetMapping;
import com.example.orchestrator.services.ProgramaRestClientService;
import com.example.orchestrator.dto.ProgramaCodDTO;


@RestController
public class OrchestratorController {

    private ProgramaRestClientService programaRestClientService;

    public OrchestratorController(ProgramaRestClientService programaRestClientService) {
        this.programaRestClientService = programaRestClientService;
    }

    /******  Stored Procedure parser successfully.  ******/
    @GetMapping("/spc_GetProgramaByCod/{prgcod}")
    public List<ProgramaCodDTO> spc_GetProgramaByCod(
            @PathVariable String prgcod) {
        List<ProgramaCodDTO> retorno = null;
            retorno = getProgramaByCodPrograma(prgcod);

        return retorno;
    }

    private List<ProgramaCodDTO> getProgramaByCodPrograma(String prgcod) {
        return programaRestClientService.getProgramaByCod(prgcod);
    }

}

