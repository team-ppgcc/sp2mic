package com.example.orchestrator.services;


import java.util.List;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import com.example.orchestrator.dto.ProgramaCodDTO;
import org.springframework.cloud.openfeign.FeignClient;


@FeignClient(name = "programa")
public interface ProgramaRestClientService {

    @GetMapping("/get-programa-by-cod/{prgcod}")
    public List<ProgramaCodDTO> getProgramaByCod(
        @PathVariable(value = "prgcod") String prgcod);

}

