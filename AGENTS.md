# AGENTS.md — Architecture & Code Style Guide for the Codex CLI Agent (C#/.NET)

## 1) Mission & Scope

* **Mission:** Generate and refactor C# code for a **.NET** application that is simple, maintainable, testable, and scalable.
* **Out of scope:** Any Unity-specific APIs/packages, game engine hooks, or engine lifecycles. This agent targets **plain .NET** (console, services, web, libraries).

---

## 2) North Stars (What to optimize for)

* **KISS:** Prefer the simplest workable design; add complexity only when justified by clear requirements .
* **SOLID:** Keep code understandable, flexible, and maintainable by applying SOLID principles judiciously, not dogmatically .

    * **SRP:** One reason to change per class/module. Favor small, cohesive units .
    * **OCP:** Open for extension, closed for modification—prefer polymorphism over branching when adding behaviors .
    * **LSP:** Subtypes must be safely substitutable; avoid base classes that force “not implemented” members  .
    * **ISP:** Split fat interfaces into focused contracts; clients shouldn’t depend on methods they don’t use .
    * **DIP:** High-level policies depend on abstractions, not concretions; reduce tight coupling and increase cohesion  .

---

## 3) Default Architecture

Use **Clean/Hexagonal** layering to isolate business logic:

* **Domain** (pure C#): entities, value objects, domain services, interfaces (ports).
* **Application**: use-cases, orchestration, validators, DTOs.
* **Infrastructure**: adapters (persistence, HTTP, file system, messaging), DI composition root.
* **Presentation**: CLI/Web endpoints/handlers mapping to Application.

**Rules of dependency:**

* Presentation/Infrastructure → Application → Domain (one-way).
* All cross-layer communication uses **interfaces** (ports) to satisfy DIP .

**When choosing patterns (menu):**

* **Factory** to encapsulate complex creation and keep callers clean; makes extension safer than branching  .
* **Command** to capture actions (do/undo, queues, logs) and separate invocation from execution  .
* **State** when behavior varies by object state (prefer composition over sprinkling conditionals).
* **Observer (events)** to decouple publishers/subscribers for notifications and UI updates.
* **Avoid Singletons** unless truly necessary; they introduce global state, tight coupling, and testing pain  .

---

## 4) Project Structure (default for new code)

```
/src
  /ProjectName.Domain
    /Entities
    /ValueObjects
    /Abstractions   // ports (interfaces)
    /Services       // domain services (pure)
  /ProjectName.Application
    /UseCases
    /Contracts      // DTOs, commands, queries, results
    /Behaviors      // pipelines (validation, logging)
  /ProjectName.Infrastructure
    /Persistence    // EF Core / Dapper / Files
    /Http
    /Messaging
    /Configuration
    /DI             // Composition root
  /ProjectName.Cli  // or Web, Worker, etc.
    /Endpoints or /Commands
    /Mappings
/tests
  /ProjectName.Domain.Tests
  /ProjectName.Application.Tests
  /ProjectName.Infrastructure.Tests
```

---

## 5) Coding Style (C#/.NET)

* **Language features:** `nullable enable`, `async/await` end-to-end, `record` for immutable DTOs, pattern matching for clarity, file-scoped namespaces.
* **Naming:** PascalCase for types/methods, camelCase for locals/parameters, UPPER\_SNAKE\_CASE for constants. Avoid abbreviations.
* **Small, focused methods:** Keep functions short; do one thing well (aligns with SRP) .
* **Avoid giant interfaces/classes:** Split responsibilities (aligns with ISP & SRP) .
* **No magic numbers:** Use readonly constants or value objects.
* **Immutability by default** for DTOs/Value Objects; prefer pure functions in Domain.
* **Error handling:** Throw exceptions for exceptional flow; use `Result` types for expected failures in Application layer. Never swallow exceptions—log with context.
* **Logging:** Structured logging (e.g., `ILogger<T>`). Log at boundaries; avoid noisy logs in inner loops.
* **Configuration:** Use options pattern; validate at startup. No hard-coded env-specific values.

---

## 6) Dependency Injection & Composition

* Use DI container (e.g., `Microsoft.Extensions.DependencyInjection`) at the **Infrastructure** composition root to wire:

    * Application services (as interfaces),
    * Repositories/adapters (as interfaces),
    * Cross-cutting concerns (caching, logging, validation).
* **Agent rule:** Prefer **constructor injection**. Never use service locators or static singletons (DIP & testability) .

---

## 7) Data & I/O Boundaries

* **Repositories/Adapters** expose **interfaces** in Domain/Application; only Infrastructure knows implementation (EF/Dapper/HTTP/etc.) (DIP) .
* **DTOs vs Entities:** Do not leak persistence models to Domain. Map explicitly.
* **Validation:** Input validation in Application (command/query validators). Domain invariants enforced inside entities/value objects.

---

## 8) Concurrency & Performance

* Use `async` everywhere for I/O; avoid blocking waits.
* Consider **object pooling** only in specialized hot paths where allocations are proven bottlenecks; encapsulate via abstractions and measure (pattern generality inspired by pooling tradeoffs) .
* Benchmark before micro-optimizing. Prefer readability first (KISS) .

---

## 9) Pattern Playbook (How the agent decides)

When you see…

* **Multiple `if/else` choosing concrete types:** Propose/introduce a **Factory** or strategy table (OCP) .
* **Undo/redo, audit trail, deferred execution, queues:** Use **Command** objects, with an invoker and optional stacks for undo/redo  .
* **Behavior changes by state:** Introduce **State** pattern (avoid sprawling conditionals).
* **Event notifications across modules:** Use **Observer** (events/delegates) to decouple publishers/subscribers.
* **Inheritance forcing “not applicable” methods:** Refactor toward **composition** and/or split interfaces (LSP/ISP)  .
* **Concrete dependency from high-level to low-level:** Insert an **interface/abstraction** (DIP) and adapt in Infrastructure .

**Singletons:** Only if a truly singular resource is unavoidable; prefer DI-scoped lifetimes instead because singletons harm testability and increase coupling .

---

## 10) Testing & Quality

* **Testing pyramid:** Unit (fast, pure), then integration (adapters), then a light layer of end-to-end.
* **Unit tests:** One assert per test conceptually; isolate via interfaces; avoid hitting real I/O.
* **Given/When/Then** naming; deterministic tests.
* **Command/Factory tests:** Verify behavior via interfaces (OCP-friendly) and undo logic where applicable .

---

## 11) CLI Interaction Patterns

* **Command handlers** map CLI verbs/options → Application use-cases.
* Keep handlers thin: parse/validate input → call use-case → render output.
* Do not embed domain logic in the CLI layer. Return meaningful exit codes.

---

## 12) Refactoring Rules the Agent Follows

* **Shrink monoliths:** Split large classes by responsibility (SRP) .
* **Replace branching with polymorphism:** Strategies/factories for extensibility (OCP) .
* **Break fat interfaces:** Extract focused interfaces (ISP) .
* **Invert dependencies:** Introduce ports (interfaces) and move concretes to Infrastructure (DIP) .
* **Prefer composition to inheritance** when real-world taxonomies conflict with substitutability (LSP) .

---

## 13) Pull Request Checklist (for generated/modified code)

* [ ] Clear, descriptive names; no dead code or commented-out blocks.
* [ ] Methods small and single-purpose; no flag parameters (split responsibilities).
* [ ] No direct references from high-level policies to concrete infrastructure (DIP).
* [ ] No fat interfaces; consumers only depend on what they use (ISP).
* [ ] Extensibility achieved without modifying stable code paths (OCP).
* [ ] Tests updated/added; fast and deterministic.
* [ ] Logging & error handling appropriate; no swallowed exceptions.
* [ ] No Unity APIs (this is a .NET app).

---

## 14) Example Stubs (Agent can generate)

### Factory (creation without branching)

```csharp
public interface IReportExporter { Task ExportAsync(Stream output, object model, CancellationToken ct); }

public sealed class CsvExporter : IReportExporter { /* ... */ }
public sealed class JsonExporter : IReportExporter { /* ... */ }

public interface IReportExporterFactory { IReportExporter Create(string format); }

public sealed class ReportExporterFactory : IReportExporterFactory
{
    private readonly IReadOnlyDictionary<string, Func<IReportExporter>> _map =
        new Dictionary<string, Func<IReportExporter>>(StringComparer.OrdinalIgnoreCase)
        {
            ["csv"] = () => new CsvExporter(),
            ["json"] = () => new JsonExporter(),
        };

    public IReportExporter Create(string format) =>
        _map.TryGetValue(format, out var ctor)
            ? ctor()
            : throw new NotSupportedException($"Format '{format}' not supported.");
}
```

*(Factory avoids `switch` sprawl and keeps callers closed to change, open to new formats) .*

### Command (do/undo capable action)

```csharp
public interface ICommand { Task ExecuteAsync(CancellationToken ct); Task UndoAsync(CancellationToken ct); }

public sealed class RenameFileCommand(string src, string dest) : ICommand
{
    public async Task ExecuteAsync(CancellationToken ct) => await Task.Run(() => File.Move(src, dest, overwrite: false), ct);
    public async Task UndoAsync(CancellationToken ct)   => await Task.Run(() => File.Move(dest, src, overwrite: false), ct);
}

public static class CommandInvoker
{
    private static readonly Stack<ICommand> _undo = new();

    public static async Task ExecuteAsync(ICommand cmd, CancellationToken ct)
    {
        await cmd.ExecuteAsync(ct);
        _undo.Push(cmd);
    }

    public static async Task UndoAsync(CancellationToken ct)
    {
        if (_undo.Count == 0) return;
        var cmd = _undo.Pop();
        await cmd.UndoAsync(ct);
    }
}
```

*(Encapsulates actions and supports undo stacks cleanly)  .*

---

## 15) When in Doubt

* Prefer the **simplest** design that meets requirements (KISS) .
* Reach for **abstractions & composition** before inheritance (LSP/ISP/DIP)   .
* Add patterns only when they **pay for themselves** in clarity or extensibility (don’t pattern-hunt) .

---

*This guide distills proven design principles and pattern trade-offs (KISS, SOLID; Factory, Command; composition over inheritance; caution with singletons) adapted for a .NET (non-Unity) context, drawing on well-known software design foundations*    .
