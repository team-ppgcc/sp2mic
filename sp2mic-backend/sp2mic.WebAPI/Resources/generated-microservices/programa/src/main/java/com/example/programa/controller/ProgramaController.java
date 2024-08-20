package com.example.programa.controller;


import com.example.programa.repository.ProgramaRepository;
import org.springframework.web.bind.annotation.RestController;
import java.util.List;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import com.example.programa.dto.ProgramaCodDTO;

import lombok.RequiredArgsConstructor;

@RestController
@RequiredArgsConstructor
public class ProgramaController {

    private final ProgramaRepository repository;

    @GetMapping("/get-programa-by-cod/{prgcod}")
    public List<ProgramaCodDTO> getProgramaByCod(
        @PathVariable(value = "prgcod") String prgcod) {
        return repository.getProgramaByCod(prgcod);
    }

}

