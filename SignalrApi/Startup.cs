using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalrApi.Hubs;
using SignalrApi.TokenHelper;
using SignalrApi.User;

namespace SignalrApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT��Ȩtokenǰ����Ҫ�����ֶ�Bearer��һ���ո�,��Bearer token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                // ��ȡxml�ļ���
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                c.IncludeXmlComments(xmlPath, true);
            });
            #endregion

            #region==signalR���������==
            services.AddCors(options => options.AddPolicy("SignalrCore",
            builder =>
            {
                var corsurls = Configuration.GetSection("SignalrCors")["default"].Split(',');
                builder.WithOrigins(corsurls)
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowCredentials();
            }));


            #endregion

            #region ==��Ȩ��֤==
            services.Configure<TokenManagement>(Configuration.GetSection("TokenManagement"));
            var token = Configuration.GetSection("TokenManagement").Get<TokenManagement>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                    ClockSkew = TimeSpan.Zero,//У��ʱ���Ƿ����ʱ�����õ�ʱ��ƫ����
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey

                };
                options.Events = new JwtBearerEvents
                {
                    //��֤ǰ
                    OnMessageReceived = (context) => {
                        if (!context.HttpContext.Request.Path.HasValue)
                        {
                            return Task.CompletedTask;
                        }
                        //�ص���������ж���Signalr��·��
                        var accessToken = context.HttpContext.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!(string.IsNullOrWhiteSpace(accessToken)) && path.StartsWithSegments("/chatHub"))
                        {
                            context.Token = accessToken;
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    },
                    //��׽���ƹ���
                    OnAuthenticationFailed = (context) =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("act", "expired");
                        }
                        return Task.CompletedTask;
                    },
                    //��֤ʧ��
                    OnChallenge = (context) => {
                        //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ�����������ҪŶ������
                        context.HandleResponse();
                        //�Զ����Լ���Ҫ���ص����ݽ����������Ҫ���ص���Json����ͨ������Newtonsoft.Json�����ת��
                        var payload = new { StatusCode = 0, Message = "�����֤ʧ�ܣ�" };
                        //�Զ��巵�ص���������
                        context.Response.ContentType = "application/json";
                        //�Զ��巵��״̬�룬Ĭ��Ϊ401
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //���Json���ݽ��
                        context.Response.WriteAsync(Convert.ToString(payload));
                        return Task.FromResult(0);
                    }
                };
            });
            #endregion

            #region==Redis==
            services.AddDistributedRedisCache(option => {
                var redisip = Configuration.GetSection("Redis")["ip"];
                var redisname = Configuration.GetSection("Redis")["name"];
                option.Configuration = redisip;
                option.InstanceName = redisname;
            });
            #endregion

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAuthenticateService, TokenAuthenticationService>();


            services.AddSignalR()
               .AddJsonProtocol(options => {
                   options.PayloadSerializerOptions.PropertyNamingPolicy = null;
               });
            //
            services.AddControllers();

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ���Swagger�й��м��
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
            });

            app.UseCors("SignalrCore");

            app.UseRouting();

            //1.�ȿ�����֤
            app.UseAuthentication();
            //2.�ٿ�����Ȩ
            app.UseAuthorization();

            //app.UseMiddleware<TokenAuth>();//TokenAuth��ע��Ϊ�м��


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                 //.RequireCors("SignalrCore");
                endpoints.MapControllers();
            });
        }
    }
}
