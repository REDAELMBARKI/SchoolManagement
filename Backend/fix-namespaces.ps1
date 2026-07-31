$ErrorActionPreference = 'Stop'
$backend = Split-Path -Parent $MyInvocation.MyCommand.Path

function Set-NamespaceFromPath {
    param(
        [string]$FilePath,
        [string]$ProjectRoot,
        [string]$RootNamespace
    )

    $relative = $FilePath.Substring($ProjectRoot.Length).TrimStart('\', '/')
    $parts = $relative -split '[\\/]'
    if ($parts[-1] -match '\.cs$') { $parts = $parts[0..($parts.Length - 2)] }
    if ($parts.Count -eq 0) { return }

    $namespace = "$RootNamespace." + ($parts -join '.')
    $content = [System.IO.File]::ReadAllText($FilePath)
    $updated = [regex]::Replace($content, 'namespace\s+[\w\.]+(\s*;|\s*\{)', "namespace $namespace`$1", 1)
    if ($updated -ne $content) {
        [System.IO.File]::WriteAllText($FilePath, $updated)
    }
}

function Replace-InFile {
    param([string]$Path, [string]$Old, [string]$New)
    if (-not (Test-Path $Path)) { return $false }
    $content = [System.IO.File]::ReadAllText($Path)
    if ($content.IndexOf($Old, [StringComparison]::Ordinal) -lt 0) { return $false }
    $content = $content.Replace($Old, $New)
    [System.IO.File]::WriteAllText($Path, $content)
    return $true
}

# 1. Align namespace declarations with folder structure
$namespaceProjects = @(
    @{ Path = Join-Path $backend 'SchoolManagement.Domain'; Root = 'SchoolManagement.Domain' },
    @{ Path = Join-Path $backend 'SchoolManagement.Application'; Root = 'SchoolManagement.Application' },
    @{ Path = Join-Path $backend 'SchoolManagement.Infrastructure'; Root = 'SchoolManagement.Infrastructure' }
)

foreach ($project in $namespaceProjects) {
    Get-ChildItem -Path $project.Path -Filter '*.cs' -Recurse | ForEach-Object {
        if ($project.Root -eq 'SchoolManagement.Application' -and $_.FullName -match '[\\/]Options[\\/]') { return }
        if ($project.Root -eq 'SchoolManagement.Infrastructure' -and $_.FullName -match '[\\/]Data[\\/]') { return }
        Set-NamespaceFromPath -FilePath $_.FullName -ProjectRoot ($project.Path + '\') -RootNamespace $project.Root
    }
}

# 2. Safe global using/namespace replacements across Backend
$allCsFiles = Get-ChildItem -Path $backend -Filter '*.cs' -Recurse
$safeReplacements = [ordered]@{
    'using SchoolManagement.Domain.Entities.EnrollmentAggregate;' = 'using SchoolManagement.Domain.Core.Entities;'
    'using SchoolManagement.Domain.Exceptions;' = 'using SchoolManagement.Domain.Common.Exceptions;'
    'using SchoolManagement.Domain.Interfaces.Repositories.Common;' = 'using SchoolManagement.Domain.Common.Interfaces;'
    'using SchoolManagement.Domain.ValueObjects;' = 'using SchoolManagement.Domain.Common.ValueObjects;'
    'using SchoolManagement.Domain.DomainEvents.Students;' = 'using SchoolManagement.Domain.Core.DomainEvents;'
    'using SchoolManagement.Infrastructure.Services.AuditLogs;' = 'using SchoolManagement.Infrastructure.Common.Services;'
    'using SchoolManagement.Application.Services.Students;' = 'using SchoolManagement.Application.Core.Services;'
    'using SchoolManagement.Application.Stratigies.LeadSourceExistence;' = 'using SchoolManagement.Application.Core.Strategies;'
    'namespace SchoolManagement.Application.Stratigies.LeadSourceExistence' = 'namespace SchoolManagement.Application.Core.Strategies'
    'namespace SchoolManagement.Domain.DomainEvents.Students;' = 'namespace SchoolManagement.Domain.Core.DomainEvents;'
    'namespace SchoolManagement.Domain.ValueObjects' = 'namespace SchoolManagement.Domain.Common.ValueObjects'
    'namespace SchoolManagement.Domain.Entities' = 'namespace SchoolManagement.Domain.Common.Entities'
}

foreach ($file in $allCsFiles) {
    foreach ($pair in $safeReplacements.GetEnumerator()) {
        Replace-InFile -Path $file.FullName -Old $pair.Key -New $pair.Value | Out-Null
    }
}

# 3. Expand legacy umbrella usings into module-specific usings
$umbrellaReplacements = [ordered]@{
    'using SchoolManagement.Domain.Entities;' = @(
        'using SchoolManagement.Domain.Academic.Entities;',
        'using SchoolManagement.Domain.Core.Entities;',
        'using SchoolManagement.Domain.Common.Entities;'
    ) -join "`r`n"
    'using SchoolManagement.Domain.Interfaces.Repositories;' = @(
        'using SchoolManagement.Domain.Academic.Interfaces;',
        'using SchoolManagement.Domain.Core.Interfaces;',
        'using SchoolManagement.Domain.Common.Interfaces;'
    ) -join "`r`n"
    'using SchoolManagement.Domain.Interfaces.Queries.Common;' = 'using SchoolManagement.Application.Common.Interfaces.Queries;'
    'using SchoolManagement.Domain.Interfaces.Queries;' = @(
        'using SchoolManagement.Application.Academic.Interfaces.Queries;',
        'using SchoolManagement.Application.Core.Interfaces.Queries;',
        'using SchoolManagement.Application.Common.Interfaces.Queries;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Dtos.Commands;' = @(
        'using SchoolManagement.Application.Academic.Dtos.Commands;',
        'using SchoolManagement.Application.Core.Dtos.Commands;',
        'using SchoolManagement.Application.Common.Dtos.Commands;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Dtos.Requests;' = @(
        'using SchoolManagement.Application.Academic.Dtos.Requests;',
        'using SchoolManagement.Application.Core.Dtos.Requests;',
        'using SchoolManagement.Application.Common.Dtos.Requests;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Dtos.Responses;' = @(
        'using SchoolManagement.Application.Academic.Dtos.Responses;',
        'using SchoolManagement.Application.Core.Dtos.Responses;',
        'using SchoolManagement.Application.Common.Dtos.Responses;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Mappers;' = @(
        'using SchoolManagement.Application.Academic.Mappers;',
        'using SchoolManagement.Application.Core.Mappers;',
        'using SchoolManagement.Application.Common.Mappers;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Interfaces.Services;' = @(
        'using SchoolManagement.Application.Academic.Interfaces.Services;',
        'using SchoolManagement.Application.Core.Interfaces.Services;',
        'using SchoolManagement.Application.Common.Interfaces.Services;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Interfaces;' = 'using SchoolManagement.Application.Common.Interfaces;'
    'using SchoolManagement.Application.Validators;' = 'using SchoolManagement.Application.Core.Validators;'
    'using SchoolManagement.Application.Interfaces.Queries;' = @(
        'using SchoolManagement.Application.Academic.Interfaces.Queries;',
        'using SchoolManagement.Application.Core.Interfaces.Queries;',
        'using SchoolManagement.Application.Common.Interfaces.Queries;'
    ) -join "`r`n"
    'using SchoolManagement.Infrastructure.Repositories;' = @(
        'using SchoolManagement.Infrastructure.Academic.Repositories;',
        'using SchoolManagement.Infrastructure.Core.Repositories;',
        'using SchoolManagement.Infrastructure.Common.Repositories;'
    ) -join "`r`n"
    'using SchoolManagement.Infrastructure.Queries;' = @(
        'using SchoolManagement.Infrastructure.Academic.Queries;',
        'using SchoolManagement.Infrastructure.Core.Queries;',
        'using SchoolManagement.Infrastructure.Common.Queries;'
    ) -join "`r`n"
    'using SchoolManagement.Application.Services;' = @(
        'using SchoolManagement.Application.Academic.Services;',
        'using SchoolManagement.Application.Core.Services;'
    ) -join "`r`n"
    'using SchoolManagement.Application.EventsHandlers.Students;' = 'using SchoolManagement.Application.Core.EventsHandlers;'
    'using SchoolManagement.Application.Results;' = 'using SchoolManagement.Application.Core.Results;'
}

foreach ($file in $allCsFiles) {
    foreach ($pair in $umbrellaReplacements.GetEnumerator()) {
        Replace-InFile -Path $file.FullName -Old $pair.Key -New $pair.Value | Out-Null
    }
}

Write-Host 'Namespace migration complete.'
