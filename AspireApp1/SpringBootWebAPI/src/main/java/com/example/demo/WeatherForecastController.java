package com.example.demo;

import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.time.LocalDate;
import java.util.List;
import java.util.Random;
import java.util.stream.IntStream;

@RestController
@CrossOrigin
public class WeatherForecastController {

    private static final String[] SUMMARIES = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    @GetMapping("/weatherforecast")
    public List<WeatherForecast> getWeatherForecast() {
        Random random = new Random();
        return IntStream.rangeClosed(1, 5)
                .mapToObj(index -> new WeatherForecast(
                        LocalDate.now().plusDays(index),
                        random.nextInt(75) - 20,
                        SUMMARIES[random.nextInt(SUMMARIES.length)]
                ))
                .toList();
    }
}
