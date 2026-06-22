<div align="center">

<h1>Senswave Backend</h1>
<p>API set for managing your DIY devices.</p>

</div>

## Stats

[![Build](https://github.com/SenswaveLabs/Backend/actions/workflows/dev-tests.yaml/badge.svg)](https://github.com/SenswaveLabs/Backend/actions/workflows/dev-tests.yaml)
[![Latest Release](https://img.shields.io/github/v/release/SenswaveLabs/Backend?label=version)](https://github.com/SenswaveLabs/Backend/releases)
[![Open Issues](https://img.shields.io/github/issues/SenswaveLabs/Backend)](https://github.com/SenswaveLabs/Backend/issues)
[![Stars](https://img.shields.io/github/stars/SenswaveLabs/Backend?style=flat)](https://github.com/SenswaveLabs/Backend/stargazers)
[![Commits](https://img.shields.io/github/commit-activity/m/SenswaveLabs/Backend)](https://github.com/SenswaveLabs/Backend/commits/main)
[![License](https://img.shields.io/github/license/SenswaveLabs/Backend)](LICENSE)


## Built With

![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white&style=flat)
![.NET 10](https://img.shields.io/badge/.NET_9-5C2D91?logo=dotnet&logoColor=white&style=flat)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-0089D6?logo=dotnet&logoColor=white&style=flat)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?logo=postgresql&logoColor=white&style=flat)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?logo=rabbitmq&logoColor=white&style=flat)
![MQTT](https://img.shields.io/badge/MQTT-3C5280?logo=eclipsemosquitto&logoColor=white&style=flat)
![SignalR](https://img.shields.io/badge/SignalR-5C2D91?logo=dotnet&logoColor=white&style=flat)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-0089D6?logo=dotnet&logoColor=white&style=flat)

## What is Senswave?

Senswave is a self-hosted and cloud backend platform for managing DIY smart home devices over MQTT. You organize your home into rooms, register custom devices with typed operations, and stream real-time data to dashboards.

**Core capabilities:**

- **Device control** — define custom operations and push commands to any MQTT-capable device
- **Automations** — trigger actions when sensor values cross thresholds or room occupancy changes
- **Live updates** — subscribe to device state via SignalR for real-time dashboard widgets
- **Home sharing** — invite users with owner or read-only access
- **Persistent broker** — server-side MQTT connection keeps automations running when your phone is off

In this repository you will find backend solution that exposes .NET HTTP Web Api with SignalR events.

## Example Use Cases

### Control a custom LED controller

A Raspberry Pi Pico LED controller supports four modes: white, color, RGB, and loading. Through Senswave you switch modes and adjust brightness from a dashboard widget — no manual MQTT publish needed.
### Auto-off lights when room is empty

A presence sensor in each room publishes occupancy data. An automation rule turns off all lights in the room when occupancy drops to zero — saves energy without any manual action.

### Always-on automations without keeping your phone open
The Senswave worker holds a persistent MQTT connection server-side. Automations fire even when no client is connected, so your phone battery is not drained keeping a session alive.

## How to Use

The easiest way to run Senswave locally is with Docker. See [`docker/README.md`](docker/README.md) for full setup instructions, including environment variables, service configuration, and how to bring up the full stack with Docker Compose.

## Contributing

Pull requests are welcome.

- Read [`AGENTS.md`](AGENTS.md) for architecture concepts and development conventions before writing code.
- Report bugs using the **Bug report** issue template.
- Propose features using the **Feature request** issue template.

## License

Distributed under the **Apache License 2.0**. See [`LICENSE`](LICENSE) for details.
