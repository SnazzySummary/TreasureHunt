using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using TreasureHunt.Models;

namespace TreasureHunt.Services;

/* Service class for performing authentication. */
public class GenerateJWTToken
{
  private IJwtAlgorithm _algorithm;
  private IJsonSerializer _serializer;
  private IBase64UrlEncoder _base64Encoder;
  private IJwtEncoder _jwtEncoder;
  public GenerateJWTToken()
  {
    // JWT specific initialization.
    _algorithm = new HMACSHA256Algorithm();
    _serializer = new JsonNetSerializer();
    _base64Encoder = new JwtBase64UrlEncoder();
    _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
  }

  /* Creates the token and stores encrypted information in it */
  public string IssuingJWT(User user)
  {
    Dictionary<string, object> claims = new Dictionary<string, object> {
            // JSON representation of the user Reference with ID and display name
            {
                "username",
                user.Username
            },
            {
                "userId",
                user.UserId
            }
        };
    string token = _jwtEncoder.Encode(claims, Environment.GetEnvironmentVariable("TokenSecurityString"));
    return token;
  }
}