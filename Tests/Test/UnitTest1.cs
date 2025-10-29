using Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;


//Verifica que la ruta /Devops esté correctamente mapeada
//Confirma que el método HTTP POST funciona

namespace Tests
{
    //public class UnitTest1 : IClassFixture<TestWebApplicationFactory>
    //{
    //    private readonly HttpClient _client;

    //    public UnitTest1(TestWebApplicationFactory factory)
    //    {
    //        _client = factory.CreateClient();
    //    }


    //    [Fact]
    //    public async Task SendMsj_WithValidRequest_ReturnsSuccess()
    //    {
    //        try
    //        {
    //            // Arrange
    //            var request = new
    //            {
    //                Message = "Test message",
    //                To = "Juan Perez",
    //                From = "Rita Asturia",
    //                TimeToLifeSec = 45
    //            };

    //            // Act
    //            var response = await _client.PostAsJsonAsync("/DevOps", request);


    //            // Assert
    //            response.EnsureSuccessStatusCode();
    //            var result = await response.Content.ReadFromJsonAsync<MessageResponse>();
    //            Assert.NotNull(result);
    //            Assert.Equal("Message sent successfully", result.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Exception: {ex}");
    //            throw;
    //        }
    //    }



    //}

    //public class UnitTest2 : IClassFixture<TestWebApplicationFactory>
    //{
    //    private readonly HttpClient _client;
    //    private readonly string _jwtToken;

    //    public UnitTest2(TestWebApplicationFactory factory)
    //    {
    //        _client = factory.CreateClient();
    //        _jwtToken = GenerateTestToken();
    //    }

    //    private string GenerateTestToken()
    //    {
    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2f5ae96c-b558-4c7b-a590-a501ae1c3f6c"));
    //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //        var token = new JwtSecurityToken(
    //            issuer: "identity",
    //            audience: "clients",
    //            claims: new[] {
    //            new Claim("sub", "test-user"),
    //            new Claim("name", "Test User")
    //            },
    //            expires: DateTime.Now.AddHours(1),
    //            signingCredentials: credentials
    //        );

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }

    //    [Fact]
    //    public async Task SendMsj_WithValidRequest_ReturnsSuccess()
    //    {
    //        try
    //        {
    //            // Arrange
    //            var request = new
    //            {
    //                Message = "Test message",
    //                To = "Juan Perez",
    //                From = "Rita Asturia",
    //                TimeToLifeSec = 45
    //            };

    //            // Agregar header de autorización
    //            _client.DefaultRequestHeaders.Authorization =
    //                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

    //            // Act
    //            var response = await _client.PostAsJsonAsync("/DevOps", request);

    //            // Assert
    //            response.EnsureSuccessStatusCode();
    //            var result = await response.Content.ReadFromJsonAsync<MessageResponse>();
    //            Assert.NotNull(result);
    //            Assert.Equal("Message sent successfully", result.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Exception: {ex}");
    //            throw;
    //        }
    //    }
    //}

    public class UnitTest2 : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _jwtToken;

        public UnitTest2(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _jwtToken = GenerateTestToken();
        }

        private string GenerateTestToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2f5ae96c-b558-4c7b-a590-a501ae1c3f6c"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "identity",
                audience: "clients",
                claims: new[] {
                new Claim("sub", "test-user"),
                new Claim("name", "Test User")
                },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task SendMsj_WithValidRequest_ReturnsSuccess()
        {
            try
            {
                // Arrange 
                var request = new
                {
                    message = "This is a test",  
                    to = "Juan Perez",
                    from = "Rita Asturia",
                    timeToLifeSec = 45          
                };

               
                _client.DefaultRequestHeaders.Clear(); 
                _client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "2f5ae96c-b558-4c7b-a590-a501ae1c3f6c");
                _client.DefaultRequestHeaders.Add("X-JWT-KWY", _jwtToken);


                // Act
                var response = await _client.PostAsJsonAsync("/DevOps", request);

                // Assert 
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<MessageResponse>();
                Assert.NotNull(result);


                Assert.Equal("Hello Juan Perez your message will be send", result.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                throw;
            }
        }

        //  TEST ADICIONAL: Verificar que otros métodos HTTP retornen "ERROR"
        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        public async Task OtherHttpMethods_ReturnError(string httpMethod)
        {
            // Arrange
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "2f5ae96c-b558-4c7b-a590-a501ae1c3f6c");
            _client.DefaultRequestHeaders.Add("X-JWT-KWY", _jwtToken);

            // Act
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/DevOps");
            var response = await _client.SendAsync(request);

            // Assert - Debe retornar "ERROR" como string
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("ERROR", content);
        }

        //  TEST: Verificar que sin API Key retorna error
        [Fact]
        public async Task WithoutApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var request = new
            {
                message = "Test message",
                to = "Juan Perez",
                from = "Rita Asturia",
                timeToLifeSec = 45
            };

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-JWT-KWY", _jwtToken); // Solo JWT, sin API Key

            // Act
            var response = await _client.PostAsJsonAsync("/DevOps", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        //  TEST: Verificar que sin JWT retorna error
        [Fact]
        public async Task WithoutJWT_ReturnsUnauthorized()
        {
            // Arrange
            var request = new
            {
                message = "Test message",
                to = "Juan Perez",
                from = "Rita Asturia",
                timeToLifeSec = 45
            };

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "2f5ae96c-b558-4c7b-a590-a501ae1c3f6c"); // Solo API Key, sin JWT

            // Act
            var response = await _client.PostAsJsonAsync("/DevOps", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

}