using System.Globalization;
using Microsoft.EntityFrameworkCore;
using StudentMarksPredictor.Data.Models;
using NN = StudentMarksPredictor.NeuralNetwork;

namespace StudentMarksPredictor.Data;

public class Repository
{
    private readonly AppDbContext _db;

    public Repository(AppDbContext db)
    {
        _db = db;
    }

    public async Task SeedFromCsvAsync(string csvPath)
    {
        if (await _db.TrainingRecords.AnyAsync())
            return;

        var lines = await File.ReadAllLinesAsync(csvPath);
        var records = lines.Skip(1).Select(line =>
        {
            var parts = line.Split(',');
            return new TrainingRecord
            {
                NumberCourses = double.Parse(parts[0], CultureInfo.InvariantCulture),
                TimeStudy = double.Parse(parts[1], CultureInfo.InvariantCulture),
                Marks = double.Parse(parts[2], CultureInfo.InvariantCulture)
            };
        });

        _db.TrainingRecords.AddRange(records);
        await _db.SaveChangesAsync();
    }

    public async Task<List<TrainingRecord>> GetAllTrainingRecordsAsync()
    {
        return await _db.TrainingRecords.ToListAsync();
    }

    public async Task AddTrainingRecordsAsync(List<TrainingRecord> records)
    {
        _db.TrainingRecords.AddRange(records);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetTrainingRecordCountAsync()
    {
        return await _db.TrainingRecords.CountAsync();
    }

    public async Task<TrainingSession> SaveSessionAsync(
        NN.NeuralNetwork network, NN.Normalizer normalizer,
        int epochs, double learningRate, double trainMSE, double testMSE)
    {
        var session = new TrainingSession
        {
            Epochs = epochs,
            LearningRate = learningRate,
            TrainMSE = trainMSE,
            TestMSE = testMSE,
            HiddenSize = network.HiddenSize,
            InputMin0 = normalizer.InputMin[0],
            InputMax0 = normalizer.InputMax[0],
            InputMin1 = normalizer.InputMin[1],
            InputMax1 = normalizer.InputMax[1],
            OutputMin = normalizer.OutputMin,
            OutputMax = normalizer.OutputMax
        };

        session.Weights = network.ExportWeights().Select(w => new ModelWeight
        {
            Layer = w.Layer,
            Row = w.Row,
            Col = w.Col,
            Value = w.Value
        }).ToList();

        _db.TrainingSessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<List<TrainingSession>> GetAllSessionsAsync()
    {
        return await _db.TrainingSessions
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<TrainingSession?> GetLatestSessionAsync()
    {
        return await _db.TrainingSessions
            .Include(s => s.Weights)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public (NN.NeuralNetwork network, NN.Normalizer normalizer) LoadModel(TrainingSession session)
    {
        var network = new NN.NeuralNetwork(2, session.HiddenSize);
        network.ImportWeights(session.Weights.Select(w =>
            new NN.WeightEntry(w.Layer, w.Row, w.Col, w.Value)).ToList());

        var normalizer = new NN.Normalizer
        {
            InputMin = new[] { session.InputMin0, session.InputMin1 },
            InputMax = new[] { session.InputMax0, session.InputMax1 },
            OutputMin = session.OutputMin,
            OutputMax = session.OutputMax
        };

        return (network, normalizer);
    }
}
