using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Email;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Jwt;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Network.Cathedral;
using ScheduleManagement.Api.Network.ContactMessage;
using ScheduleManagement.Api.Network.Department;
using ScheduleManagement.Api.Network.Helper;
using ScheduleManagement.Api.Network.LastOpenedSchedules;
using ScheduleManagement.Api.Network.MemoryStorage;
using ScheduleManagement.Api.Network.Profile;
using ScheduleManagement.Api.Network.ResetPassword;
using ScheduleManagement.Api.Network.ScheduleSubject;
using ScheduleManagement.Api.Network.SearchContent;
using ScheduleManagement.Api.Network.StudyGroup;
using ScheduleManagement.Api.Network.StudyRoom;
using ScheduleManagement.Api.Network.StudySpec;
using ScheduleManagement.Api.Network.StudySubject;
using ScheduleManagement.Api.Network.TimeManagement;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;
using ScheduleManagement.Api.S3;
using ScheduleManagement.Api.Ssh;

var builder = WebApplication.CreateBuilder(args);

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// bind configuration
ApiConfig.BindValuesFromConfigFile(builder);

// logging
builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddConsole(); });

// validation and JSON
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// auth
builder.Services.AddSingleton<IJwtAuthManager>(new JwtAuthManagerImpl());
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
	options.SaveToken = true;
	options.TokenValidationParameters = JwtAuthManagerImpl.GetBasicTokenValidationParameters();
});
builder.Services.AddScoped<IPasswordHasher<Person>, PasswordHasher<Person>>();

// services
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();
builder.Services.AddScoped<ICathedralService, CathedralServiceImpl>();
builder.Services.AddScoped<IContactMessageService, ContactMessageServiceImpl>();
builder.Services.AddScoped<IDepartmentService, DepartmentServiceImpl>();
builder.Services.AddScoped<IProfileService, ProfileServiceImpl>();
builder.Services.AddScoped<IHelperService, HelperServiceImpl>();
builder.Services.AddScoped<IResetPasswordService, ResetPasswordServiceImpl>();
builder.Services.AddScoped<IScheduleSubjectService, ScheduleSubjectServiceImpl>();
builder.Services.AddScoped<ISearchContentService, SearchContentServiceImpl>();
builder.Services.AddScoped<IStudyGroupService, StudyGroupServiceImpl>();
builder.Services.AddScoped<IStudyRoomService, StudyRoomServiceImpl>();
builder.Services.AddScoped<IStudySpecService, StudySpecServiceImpl>();
builder.Services.AddScoped<IStudySubjectService, StudySubjectServiceImpl>();
builder.Services.AddScoped<ITimeManagementService, TimeManagementServiceImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<IMemoryStorageService, MemoryStorageServiceImpl>();
builder.Services.AddScoped<ILastOpenedSchedulesService, LastOpenedSchedulesServiceImpl>();

builder.Services.AddScoped<IS3Service, S3ServiceImpl>();
builder.Services.AddScoped<ISshInterceptor, SshInterceptorImpl>();
builder.Services.AddScoped<IMailboxProxyService, MailboxProxyServiceImpl>();
builder.Services.AddScoped<IMailSenderService, MailSenderServiceImpl>();

// middlewares
builder.Services.AddScoped<ExceptionMiddlewareHandler>();
builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));

// validators
builder.Services.AddScoped<IValidator<SearchQueryRequestDto>, UserQueryValidator>();
builder.Services.Configure<FormOptions>(o =>
{
	o.ValueLengthLimit = int.MaxValue;
	o.MultipartBodyLengthLimit = int.MaxValue;
	o.MemoryBufferThreshold = int.MaxValue;
});

// database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(
		builder.Configuration.GetConnectionString("MySQL"),
		new MySqlServerVersion(ApiConfig.DbDriverVersion),
		opt =>
		{
			opt.EnableStringComparisonTranslations();
			opt.EnableRetryOnFailure(
				10,
				TimeSpan.FromSeconds(5),
				null);
		}
	));
builder.Services.AddScoped<ApplicationDbSeeder>();

// routing
builder.Services.AddRouting(options =>
{
	options.LowercaseUrls = true;
	options.LowercaseQueryStrings = true;
});
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(corsPolicyBuilder =>
		corsPolicyBuilder.WithOrigins(ApiConfig.ClientOrigin)
			.AllowAnyMethod()
			.AllowAnyHeader()
	);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(config => { config.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedule Management Server"); });
}

app.UseCors();

using (var scope = app.Services.CreateScope())
{
	var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
	seeder.Seed().Wait();
}

if (app.Environment.IsProduction())
{
	app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddlewareHandler>();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
