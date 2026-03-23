namespace StudentMarksPredictor.NeuralNetwork;

// simple record to hold a single weight value with its position info
// layer tells us which matrix it belongs to (IH, HO, BiasH, BiasO)
public record WeightEntry(string Layer, int Row, int Col, double Value);
