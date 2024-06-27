namespace ToDoList.DTO {
    public class TodoTask {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString() => Name ?? string.Empty;
    }

}