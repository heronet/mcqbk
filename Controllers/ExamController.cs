using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    [Authorize]
    public class ExamController : DefaultController
    {
        private readonly UserManager<EntityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public ExamController(UserManager<EntityUser> userManager, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpPost("create")]
        public async Task<ActionResult> CreateExam(CreateExamDTO createExamDTO)
        {
            // Get the currentuser/ creator
            var username = User.FindFirst(ClaimTypes.Name).Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Unauthorized("Invalid User");

            var questions = new List<Question>();
            int totalMarks = 0;
            createExamDTO.Questions.ForEach(questionDto =>
            {
                var options = new List<QuestionOption>();

                // Transform each option string to QuestionOption
                questionDto.Options.ForEach(optionDTO =>
                {
                    options.Add(new QuestionOption { Option = optionDTO.Text, HasMath = optionDTO.HasMath });
                });
                // Transform QuestionDTO to Question
                var question = new Question
                {
                    Title = questionDto.Title,
                    Options = options,
                    CorrectAnswerText = questionDto.CorrectAnswer.Text,
                    CorrectAnswerHasMath = questionDto.CorrectAnswer.HasMath,
                    Marks = questionDto.Marks
                };
                // Add question mark to total marks
                totalMarks += questionDto.Marks;
                // Add Question to List<Question> questions
                questions.Add(question);
            });
            var exam = new Exam
            {
                Title = createExamDTO.Title,
                Subject = subjectStringToEnum(createExamDTO.Subject),
                Questions = questions,
                Duration = createExamDTO.Duration,
                Creator = user,
                TotalMarks = totalMarks
            };
            _dbContext.Exams.Add(exam);
            if (await _dbContext.SaveChangesAsync() > 0)
                return Ok();
            return BadRequest("Failed To Add Exam");
        }
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetExamDTO>>> GetExams(
            [FromQuery] string myRole,
            [FromQuery] DateTime date,
            [FromQuery] int pageSize,
            [FromQuery] int pageCount,
            [FromQuery] string testId = "81a130d2-502f-4cf1-a376-63edeb000e9f"
        )
        {
            var exams = new List<Exam>();
            var examDtos = new List<GetExamDTO>();
            long examCount = 0;

            if (!string.IsNullOrWhiteSpace(testId) && (testId != "81a130d2-502f-4cf1-a376-63edeb000e9f"))
            {
                Guid id;
                if (!Guid.TryParse(testId, out id))
                {
                    return BadRequest("Invalid Exam ID");
                }
                var exam = await _dbContext.Exams
                    .Where(e => e.Id == id)
                    .Include(e => e.Creator)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
                if (exam != null)
                {
                    examDtos.Add(GetSimpleExamDto(exam));
                    return Ok(new GetExamWithPage { Exams = examDtos, Size = 1 });
                }
                return BadRequest("Invalid Exam ID");
            }
            if (myRole == "participient")
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (username != null)
                {
                    EntityUser user;

                    if (date != DateTime.MinValue)
                    {
                        user = await _userManager.Users
                        .Where(u => u.UserName == username)
                        .Include(u => u.ParticipatedExams
                            .Where(er => er.Exam.CreatedAt.Date == date)
                        )
                        .ThenInclude(er => er.Exam)
                        .ThenInclude(e => e.Creator)
                        .AsSplitQuery()
                        .SingleOrDefaultAsync();
                    }
                    else
                    {
                        user = await _userManager.Users
                        .Where(u => u.UserName == username)
                        .Include(u => u.ParticipatedExams)
                        .ThenInclude(er => er.Exam)
                        .ThenInclude(e => e.Creator)
                        .AsSplitQuery()
                        .SingleOrDefaultAsync();
                    }
                    examCount = user.ParticipatedExams.LongCount();
                    var examResults = user.ParticipatedExams.Skip(pageSize * (pageCount - 1)).Take(pageSize).ToList();
                    examResults.ForEach(examResult =>
                    {
                        // We return a simpler ExamDTO when fetching all exams
                        var exam = examResult.Exam;
                        var examDto = GetSimpleExamDto(exam);
                        examDto.MarksObtained = examResult.Score;
                        examDtos.Add(examDto);
                    });
                    return Ok(new GetExamWithPage { Exams = examDtos, Size = examCount });
                }
                else
                    return BadRequest("Not Authorized");
            }
            if (myRole == "creator")
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (username != null)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    List<Exam> createdExams;
                    if (date != DateTime.MinValue)
                    {
                        examCount = await _dbContext.Exams
                        .Where(e => e.CreatedAt.Date == date)
                        .LongCountAsync();

                        createdExams = await _dbContext.Exams
                        .Where(e => e.CreatedAt.Date == date)
                        .OrderBy(e => e.CreatedAt)
                        .Include(e => e.Creator)
                        .Where(e => e.CreatorId == user.Id)
                        .Skip(pageSize * (pageCount - 1))
                        .Take(pageSize)
                        .AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
                    }
                    else
                    {
                        examCount = await _dbContext.Exams
                        .LongCountAsync();

                        createdExams = await _dbContext.Exams
                        .Include(e => e.Creator)
                        .Where(e => e.CreatorId == user.Id)
                        .OrderBy(e => e.CreatedAt)
                        .Skip(pageSize * (pageCount - 1))
                        .Take(pageSize)
                        .AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
                    }
                    createdExams.ForEach(createdExam =>
                    {
                        // We return a simpler ExamDTO when fetching all exams
                        var examDto = GetSimpleExamDto(createdExam);
                        examDtos.Add(examDto);
                    });
                    return Ok(new GetExamWithPage { Exams = examDtos, Size = examCount });
                }
                else
                    return BadRequest("Not Authorized");
            }

            if (date != DateTime.MinValue)
            {
                examCount = await _dbContext.Exams
                        .Where(e => e.CreatedAt.Date == date)
                        .LongCountAsync();
                exams = await _dbContext.Exams
                .Where(e => e.CreatedAt.Date == date)
                .OrderBy(e => e.CreatedAt)
                .Include(exams => exams.Creator)
                .Skip(pageSize * (pageCount - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            }
            else
            {
                examCount = await _dbContext.Exams
                        .LongCountAsync();
                exams = await _dbContext.Exams
                .OrderBy(e => e.CreatedAt)
                .Include(exams => exams.Creator)
                .Skip(pageSize * (pageCount - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            }
            exams.ForEach(exam =>
            {
                // We return a simpler ExamDTO when fetching all exams
                var examDto = GetSimpleExamDto(exam);
                examDtos.Add(examDto);
            });
            return Ok(new GetExamWithPage { Exams = examDtos, Size = examCount });
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetExamDTO>> GetExam(Guid id)
        {
            var exam = await _dbContext.Exams
                .Where(e => e.Id == id)
                .Include(e => e.Participients)
                .Include(exams => exams.Creator)
                .Include(exam => exam.Questions)
                .ThenInclude(question => question.Options)
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (exam == null)
                return BadRequest("Invalid Exam Id");
            string username;
            EntityUser user;

            username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username != null)
            {
                user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.ParticipatedExams)
                .ThenInclude(er => er.Exam)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
            }
            else
                user = null;
            // Check if perticipated before
            var participatedDto = false;

            var examDto = examToDto(exam);
            if (user != null)
            {
                var participated = exam.Participients.Where(u => u.UserName == username)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

                if (participated != null)
                { // If user sits for the first time, count him as new.
                    participatedDto = true;
                    var partExam = user.ParticipatedExams.Where(er => er.Exam.Id == exam.Id).FirstOrDefault();
                    examDto.MarksObtained = partExam.Score;
                }
            }

            examDto.Participated = participatedDto;

            return Ok(examDto);
        }
        [HttpPost("submit")]
        public async Task<ActionResult<GetExamDTO>> SubmitExam(GetExamDTO getExamDTO)
        {
            var username = User.FindFirst(ClaimTypes.Name).Value;
            var user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.ParticipatedExams)
                .FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized("Invalid User");

            var exam = await _dbContext.Exams
                .Where(e => e.Id == getExamDTO.Id)
                .Include(e => e.Creator)
                .Include(e => e.Participients)
                .Include(exam => exam.Questions)
                .ThenInclude(question => question.Options)
                .AsSplitQuery()
                .SingleOrDefaultAsync();

            if (exam == null)
                return BadRequest("Invalid Exam Id");

            var marksObtained = 0;
            for (int i = 0; i != getExamDTO.Questions.Count(); ++i)
            {
                var question = exam.Questions.ElementAt(i);

                if (question.CorrectAnswerText == getExamDTO.Questions[i].ProvidedAnswer.Text)
                {
                    marksObtained += question.Marks;
                }
            }
            var participated = exam.Participients.Where(u => u.UserName == username)
                .DefaultIfEmpty(null)
                .FirstOrDefault();
            if (participated == null) // If user sits for the first time, count him as new.
            {
                exam.Participients.Add(user);
                ++exam.Attendees;
                await _dbContext.SaveChangesAsync();
                var result = new ExamResult
                {
                    Exam = exam,
                    Score = marksObtained
                };
                user.ParticipatedExams.Add(result);
                await _userManager.UpdateAsync(user);
            }
            var examDto = examToDto(exam);

            examDto.Participated = true;
            examDto.MarksObtained = marksObtained;
            examDto.Questions = getExamDTO.Questions;
            examDto.NewSubmission = true;
            return Ok(examDto);
        }

        // Utility Methods
        private GetExamDTO examToDto(Exam exam)
        {
            var questionDtos = new List<QuestionDTO>();
            foreach (var question in exam.Questions)
            {
                var options = new List<OptionDTO>();
                question.Options.ForEach(option =>
                {
                    options.Add(new OptionDTO { Text = option.Option, HasMath = option.HasMath });
                });
                var questionDto = new QuestionDTO
                {
                    Id = question.Id,
                    Title = question.Title,
                    CorrectAnswer = new OptionDTO
                    {
                        HasMath = question.CorrectAnswerHasMath,
                        Text = question.CorrectAnswerText
                    },
                    ProvidedAnswer = new OptionDTO
                    {
                        HasMath = false,
                        Text = ""
                    },
                    Options = options,
                    Marks = question.Marks
                };
                questionDtos.Add(questionDto);
            }
            var examDto = new GetExamDTO
            {
                Id = exam.Id,
                Title = exam.Title,
                Subject = subjectEnumToString(exam.Subject),
                Duration = exam.Duration,
                Questions = questionDtos,
                CreatedAt = exam.CreatedAt,
                CreatorId = exam.CreatorId,
                Creator = exam.Creator.UserName,
                TotalMarks = exam.TotalMarks,
                Attendees = exam.Attendees
            };
            return examDto;
        }
        private GetExamDTO GetSimpleExamDto(Exam exam)
        {
            return new GetExamDTO
            {
                Id = exam.Id,
                Title = exam.Title,
                CreatedAt = exam.CreatedAt,
                Duration = exam.Duration,
                Subject = subjectEnumToString(exam.Subject),
                Attendees = exam.Attendees,
                CreatorId = exam.CreatorId,
                Creator = exam.Creator.UserName,
                TotalMarks = exam.TotalMarks
            };
        }
        private Subject subjectStringToEnum(string subject)
        {
            switch (subject.ToLower())
            {
                case "english":
                    return Subject.English;
                case "bangla":
                    return Subject.Bangla;
                case "math":
                    return Subject.Math;
                case "ict":
                    return Subject.ICT;
                case "physics":
                    return Subject.Physics;
                case "chemistry":
                    return Subject.Chemistry;
                case "biology":
                    return Subject.Biology;
                default:
                    return Subject.Multi;
            }
        }
        private string subjectEnumToString(Subject subject)
        {
            switch (subject)
            {
                case Subject.English:
                    return "English";
                case Subject.Bangla:
                    return "Bangla";
                case Subject.Math:
                    return "Math";
                case Subject.ICT:
                    return "Ict";
                case Subject.Physics:
                    return "Physics";
                case Subject.Chemistry:
                    return "Chemistry";
                case Subject.Biology:
                    return "Biology";
                default:
                    return "Multiple Subjects";
            }
        }
    }
}