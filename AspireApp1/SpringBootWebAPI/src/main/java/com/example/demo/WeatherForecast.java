package com.example.demo;

import java.time.LocalDate;

public record WeatherForecast(LocalDate date, int temperatureC, String summary) {
    public int getTemperatureF() {
        return 32 + (int) (temperatureC / 0.5556);
    }
}