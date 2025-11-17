import { useEffect, useState } from "react";
import type { Incident } from "../api/incidentService";
import { IncidentService } from "../api/incidentService";
import { Link } from "react-router-dom";

export default function Dashboard() {
  const [incidents, setIncidents] = useState<Incident[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    IncidentService.getAll()
      .then((res: { data: Incident[] }) => setIncidents(res.data))
      .finally(() => setLoading(false));
  }, []);

  const stats = {
    total: incidents.length,
    open: incidents.filter((i) => i.status === "Open").length,
    inProgress: incidents.filter((i) => i.status === "InProgress").length,
    closed: incidents.filter((i) => i.status === "Closed").length,
  };

  const recentIncidents = incidents.slice(0, 5);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
          <p className="text-gray-600 text-lg">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Dashboard</h1>
        <p className="text-gray-600">Overview of your incident management system</p>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Total Incidents */}
        <div className="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg p-6 text-white">
          <div className="flex items-center justify-between mb-4">
            <div className="text-4xl">📊</div>
            <div className="text-3xl font-bold">{stats.total}</div>
          </div>
          <h3 className="text-sm font-medium opacity-90">Total Incidents</h3>
        </div>

        {/* Open Incidents */}
        <div className="bg-gradient-to-br from-amber-500 to-amber-600 rounded-xl shadow-lg p-6 text-white">
          <div className="flex items-center justify-between mb-4">
            <div className="text-4xl">🔴</div>
            <div className="text-3xl font-bold">{stats.open}</div>
          </div>
          <h3 className="text-sm font-medium opacity-90">Open</h3>
        </div>

        {/* In Progress */}
        <div className="bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg p-6 text-white">
          <div className="flex items-center justify-between mb-4">
            <div className="text-4xl">🟡</div>
            <div className="text-3xl font-bold">{stats.inProgress}</div>
          </div>
          <h3 className="text-sm font-medium opacity-90">In Progress</h3>
        </div>

        {/* Closed */}
        <div className="bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg p-6 text-white">
          <div className="flex items-center justify-between mb-4">
            <div className="text-4xl">🟢</div>
            <div className="text-3xl font-bold">{stats.closed}</div>
          </div>
          <h3 className="text-sm font-medium opacity-90">Closed</h3>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Quick Actions</h2>
        <div className="flex flex-wrap gap-4">
          <Link
            to="/create"
            className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-600 to-blue-700 text-white px-6 py-3 rounded-lg shadow-lg hover:from-blue-700 hover:to-blue-800 transition-all duration-200 transform hover:scale-105 font-medium"
          >
            <span>➕</span>
            Create New Incident
          </Link>
          <Link
            to="/incidents"
            className="inline-flex items-center gap-2 bg-white border border-gray-300 text-gray-700 px-6 py-3 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            <span>📋</span>
            View All Incidents
          </Link>
        </div>
      </div>

      {/* Recent Incidents */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-bold text-gray-900">Recent Incidents</h2>
            <Link
              to="/incidents"
              className="text-blue-600 hover:text-blue-700 font-medium text-sm"
            >
              View All →
            </Link>
          </div>
        </div>

        {recentIncidents.length === 0 ? (
          <div className="p-12 text-center">
            <div className="text-6xl mb-4">📋</div>
            <h3 className="text-lg font-semibold text-gray-900 mb-2">
              No incidents yet
            </h3>
            <p className="text-gray-600 mb-6">
              Get started by creating your first incident report.
            </p>
            <Link
              to="/create"
              className="inline-flex items-center gap-2 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium"
            >
              <span>+</span> Create Your First Incident
            </Link>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {recentIncidents.map((incident) => {
              const getStatusBadgeClass = (status: string) => {
                switch (status) {
                  case "Open":
                    return "bg-amber-100 text-amber-800 border-amber-200";
                  case "InProgress":
                    return "bg-blue-100 text-blue-800 border-blue-200";
                  case "Closed":
                    return "bg-green-100 text-green-800 border-green-200";
                  default:
                    return "bg-gray-100 text-gray-800 border-gray-200";
                }
              };

              const getStatusIcon = (status: string) => {
                switch (status) {
                  case "Open":
                    return "🔴";
                  case "InProgress":
                    return "🟡";
                  case "Closed":
                    return "🟢";
                  default:
                    return "⚪";
                }
              };

              return (
                <div
                  key={incident.id}
                  className="p-4 hover:bg-gray-50 transition-colors"
                >
                  <div className="flex items-center justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <span className="font-mono text-sm text-gray-500">
                          #{incident.id}
                        </span>
                        <span
                          className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold border ${getStatusBadgeClass(
                            incident.status
                          )}`}
                        >
                          <span>{getStatusIcon(incident.status)}</span>
                          {incident.status === "InProgress"
                            ? "In Progress"
                            : incident.status}
                        </span>
                      </div>
                      <h3 className="font-semibold text-gray-900 mb-1">
                        {incident.title}
                      </h3>
                      {incident.description && (
                        <p className="text-sm text-gray-600 line-clamp-2">
                          {incident.description}
                        </p>
                      )}
                    </div>
                    <Link
                      to={`/edit/${incident.id}`}
                      className="ml-4 text-blue-600 hover:text-blue-700 font-medium text-sm"
                    >
                      View →
                    </Link>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}

