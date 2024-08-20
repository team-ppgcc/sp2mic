package com.example.programa.controller;


import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class IndexController {

    @Value("${spring.profiles.active}")
    String zone;

    @GetMapping("/")
    public String sayHello() {
        return "programa microservice - %s".formatted(zone);
    }
}

