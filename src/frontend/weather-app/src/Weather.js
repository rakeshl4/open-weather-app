import React, { useState } from 'react';
import axios from 'axios';

const Weather = () => {
  const [error, setError] = useState('');
  const [city, setCity] = useState('');
  const [country, setCountry] = useState('');
  const [apiKey, setAPIKey] = useState('');
  const [weatherData, setWeatherData] = useState(null);

  const fetchData = async () => {
    try {

      setError(null); 
      const response = 
      await axios.get(
        `http://localhost:5150/weather?q=${city},${country}&appid=${apiKey}`
      );
      setWeatherData(response.data);
      console.log(response.data); //You can see all the weather data in console log
    } catch (error) {
      setWeatherData(null);
      setError(error.message);
      console.error(error);
    }
  };

  const handleCityChange = (e) => {
    setCity(e.target.value);
  };

  const handleCountryChange = (e) => {
    setCountry(e.target.value);
  };

  const handleAPIKeyChange = (e) => {
    setAPIKey(e.target.value);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    fetchData();
  };

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <div>
          <label>City:</label>
          <input
            type="text"
            placeholder="Enter city name"
            value={city}
            onChange={handleCityChange}
          />
        </div>
        <div>
          <label>Country:</label>
          <input
            type="text"
            placeholder="Enter country name"
            value={country}
            onChange={handleCountryChange}
          />
        </div>

        <div>
          <label>API Key:</label>
          <input
            type="text"
            placeholder="Enter API key"
            value={apiKey}
            onChange={handleAPIKeyChange}
          />
        </div>

        <button type="submit">Get Weather</button>
      </form>
      {weatherData ? (
        <> 
          <p style={{ color: 'green' }}>The weather information was fetched from the service successfully.</p>
          <p style={{ color: 'green' }}>The current weather is - `{weatherData}`</p>
        </>
      ) : (
        <p></p>
      )}
      {error ? (
         <> 
         <p style={{ color: 'red' }}>Failed to fetch the weather information - </p>
        <p style={{ color: 'red' }}>{error}</p>
        </>
      ) : (
        <p></p>
      )}
    </div>
  );
};

export default Weather;