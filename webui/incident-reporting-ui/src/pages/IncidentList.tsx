import { useEffect, useState } from "react";
import type { Incident } from "../api/incidentService";
import { IncidentService } from "../api/incidentService";
import { Link } from "react-router-dom";

export default function IncidentList() {
  const [incidents, setIncidents] = useState<Incident[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    IncidentService.getAll()
      .then((res: { data: Incident[] }) => setIncidents(res.data))
      .finally(() => setLoading(false));
  }, []);

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

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
          <p className="text-gray-600 text-lg">Loading incidents...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header Section */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 mb-2">All Incidents</h1>
          <p className="text-gray-600">
            {incidents.length === 0
              ? "No incidents found"
              : `${incidents.length} incident${incidents.length !== 1 ? "s" : ""} total`}
          </p>
        </div>

        <Link
          to="/create"
          className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-600 to-blue-700 text-white px-6 py-3 rounded-lg shadow-lg hover:from-blue-700 hover:to-blue-800 transition-all duration-200 transform hover:scale-105 font-medium"
        >
          <span className="text-xl">+</span>
          Create New Incident
        </Link>
      </div>

      {/* Incidents Table */}
      {incidents.length === 0 ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
          <div className="text-6xl mb-4">📋</div>
          <h3 className="text-xl font-semibold text-gray-900 mb-2">
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
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gradient-to-r from-gray-50 to-gray-100 border-b border-gray-200">
                <tr>
                  <th className="text-left p-4 font-semibold text-gray-700">ID</th>
                  <th className="text-left p-4 font-semibold text-gray-700">Title</th>
                  <th className="text-left p-4 font-semibold text-gray-700">Status</th>
                  <th className="text-left p-4 font-semibold text-gray-700">Actions</th>
                </tr>
              </thead>

              <tbody className="divide-y divide-gray-100">
                {incidents.map((i) => (
                  <tr
                    key={i.id}
                    className="hover:bg-blue-50/50 transition-colors duration-150"
                  >
                    <td className="p-4">
                      <span className="font-mono text-sm text-gray-600">#{i.id}</span>
                    </td>
                    <td className="p-4">
                      <div className="font-medium text-gray-900">{i.title}</div>
                      {i.categoryName && (
                        <div className="text-xs text-blue-600 mt-1 font-medium">
                          📁 {i.categoryName}
                        </div>
                      )}
                      {i.description && (
                        <div className="text-sm text-gray-500 mt-1 line-clamp-1">
                          {i.description}
                        </div>
                      )}
                    </td>
                    <td className="p-4">
                      <span
                        className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-semibold border ${getStatusBadgeClass(
                          i.status
                        )}`}
                      >
                        <span>{getStatusIcon(i.status)}</span>
                        {i.status === "InProgress" ? "In Progress" : i.status}
                      </span>
                    </td>
                    <td className="p-4">
                      <Link
                        to={`/edit/${i.id}`}
                        className="inline-flex items-center gap-1 text-blue-600 hover:text-blue-700 font-medium transition-colors"
                      >
                        <span>✏️</span>
                        Edit
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
