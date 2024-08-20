package com.example.programa.exception;

import java.util.logging.Level;
import java.util.logging.Logger;
import org.springframework.dao.EmptyResultDataAccessException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@RestControllerAdvice
public class ResourceExceptionHandler {

    private static final String MESSAGE = "MESSAGE: {0}";
    private static final String CAUSE = "CAUSE: {0}";
    private static final String STACK = "STACK: {0}";
    private final Logger logger = Logger.getLogger(ResourceExceptionHandler.class.getName());

    @ExceptionHandler({EmptyResultDataAccessException.class})
    @ResponseStatus(HttpStatus.NOT_FOUND)
    public ResponseEntity<String> emptyResultDataAccessExceptionHandler(Exception e) {
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(e.getMessage());
    }

    @ExceptionHandler({NullPointerException.class})
    @ResponseStatus(HttpStatus.BAD_REQUEST)
    public ResponseEntity<String> nullPointerExceptionHandler(Exception e) {
        logger.log(Level.SEVERE, MESSAGE, e.getMessage());
        logger.log(Level.SEVERE, CAUSE, e.getCause() == null ? "" : e.getCause());
        logger.log(Level.SEVERE, STACK, e.getStackTrace());
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("unhandled error");
    }
}

